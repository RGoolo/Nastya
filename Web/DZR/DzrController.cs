using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Model.Bots.BotTypes.Class;
using Model.Bots.BotTypes.Interfaces.Ids;
using Model.Bots.BotTypes.Interfaces.Messages;
using Model.Logic.Settings;
using Web.Base;
using Model.Logic.Model;
using Web.DZR.PageTypes;
using Web.Entitiy;

namespace Web.DZR
{
	public class DzrController : IController
	{
		private DzrPage _lastPage;
		private DzrWebValidator _dzrWebValidator;

		public DzrController(IChatService settings)
		{
			Settings = settings;
			_dzrWebValidator = new DzrWebValidator(settings);
		}

		// public event SendMsgsSyncDel SendMsgs;
		public event SendMsgDel SendMsg;
		public IChatService Settings { get; }
		public void LogIn() => _lastPage = _dzrWebValidator.LogIn().Result; //ToDo delete result

		public bool IsLogOut() => _dzrWebValidator.IsLogOut(_lastPage);

		public List<IEvent> GetCode(string str, IUser user, IMessageId replaceMsg)
		{
			var codes = GetCodes(str, Settings.DzzzrGame.Prefix?.ToLower() ?? string.Empty);

			return codes?.Select(cod => (IEvent)new SimpleEvent(EventTypes.SendCode, user, cod, replaceMsg)).ToList();
		}

		public void SendEvent(IEvent iEvent)
		{
			switch (iEvent.EventType)
			{
				case EventTypes.SendSpoiler:
					break;
				case EventTypes.SendCode:
					var page = _dzrWebValidator.SendCode(iEvent.Text, GetMainTask(_lastPage)).Result; //ToDo await
					AfterSendCode(page, iEvent);
					SetNewPage(page);
					break;
				case EventTypes.GetLvlInfo:
					SendPageInfo(_lastPage, true);
					break;
				case EventTypes.GetBonus:
					break;
				case EventTypes.GetAllSectors:
					SendSectors(_lastPage, true, true);
					break;
				case EventTypes.GetSectors:
					SendSectors(_lastPage, true, false);
					break;
				case EventTypes.GetAllInfo:
					SendPageInfo(_lastPage, false);
					break;
				case EventTypes.GetTimeForEnd:
					SendTimeToEnd();
					break;
			}
		}

		public void Refresh()
		{
			var page = _dzrWebValidator.GetNextPage().Result; //ToDo delete result
			SetNewPage(page);
		}

		public void SendTimeToEnd()
		{
			var task = GetMainTask(_lastPage);
			var time = _lastPage?.TimeToEnd?.ToString();
			var text = (task == null)? "⏳ Времени осталось: " + time : task.GetTextTimeToEnd(time);

			SendTextMsg(text);
		}


		public void AfterSendCode(DzrPage page, IEvent iEvent)
		{
			//Controller(page, )
			//var t = CommandMessage.GetTextMsg(new Texter(message, withHtml));
			//t.OnIdMessage = replaceMsgId.GetValueOrDefault();

			//Controller.SetGameAnswer( )

			SendTextMsg(page.GetAnswerText(iEvent.Text), iEvent.IdMsg, false);
		}

		public void SetNewPage(DzrPage page)
		{
			if (page == null || page.Type == PageType.NotFound)
				return;

			SendDifference(_lastPage, page);
			_lastPage = page;
			
			var lvlName = GetMainTask(_lastPage)?.LvlNumber;
			//Controller.SetNewLvl(lvlName);
		}

		private void SendDiffTime(TimeSpan? lastTime, TimeSpan? newTime)
		{
			if (BaseCheckChanges.IsBorderValue(lastTime, newTime, 60))
				SendTimeToEnd();
			if (BaseCheckChanges.IsBorderValue(lastTime, newTime, 300))
				SendTimeToEnd();
		}

		public void SendDifference(DzrPage lastPage, DzrPage newPage)
		{
			//if (newPage.AnswerType != AnswerType.None)

			var checkOtherTask = Settings.DzzzrGame.CheckOtherTask;
			if (lastPage == null || lastPage.Type != newPage.Type)
			{
				SendPageInfo(newPage, !checkOtherTask);
					return;
			}

			SendDiffTime(lastPage.TimeToEnd, newPage.TimeToEnd);

			if (!checkOtherTask)
			{ 
				SendDifference(GetMainTask(newPage), GetMainTask(lastPage), newPage.TimeToEnd);
			}
			else
			{
				foreach (var task in newPage.Tasks)
				{
					var oldTask = lastPage.Tasks?.FirstOrDefault(x => x.LvlNumber == task.LvlNumber);
					SendDifference(task, oldTask, newPage.TimeToEnd);
				}
			}
		}

		private void CheckDiffCode(DzrPage lastPage, DzrPage newPage, IUser user)
		{
			var newTask = GetMainTask(newPage);
			var oldTask = GetMainTask(lastPage);

			if (newPage.AnswerType == AnswerType.None)
			{
				if (newTask == null || newTask.LvlNumber != oldTask?.LvlNumber)
					return;

				for (int i = 0; i < newTask.Codes.Count; ++i)
				{
					if (i >= oldTask.Codes.Count)
						return;

					var diffs = newTask.Codes[i].Diff(oldTask.Codes[i]).ToList();
					if (!diffs.Any())
						continue;

					SendTextMsg($"⚠️Новые коды:\n{newTask.Codes[i].DiffText(diffs)}");

					foreach (var diff in diffs)
					{
						var answer = new Answer();
						answer.Code = diff.Answered;
						//Controller.SetGameAnswer(answer);
					}
				}
			}
			else
			{

			}
		}


		private void SendDifference(DzrTask task, DzrTask oldTask, TimeSpan? newPageTimeToEnd)
		{
			if (task == null)
				return;

			
			
			if (oldTask == null || task?.LvlNumber != oldTask?.LvlNumber)
			{
				SndMsg(MessageToBot.GetTextMsg(CheckChanges.GetTaskInfo(task, true, newPageTimeToEnd?.ToString())));
				return;
			}

			if (task.Spoilers?.Count > 0)
			{
				for (var i = 0; i < task.Spoilers.Count(); ++i)
				{
					if (!(i < oldTask.Spoilers?.Count)) continue;

					if (!oldTask.Spoilers[i].IsCompleted && task.Spoilers[i].IsCompleted)
						SndMsg(MessageToBot.GetTextMsg(new Texter($"{task.Alias}\n🔑Разгадан спойлер:\n{task.Spoilers[i].Text}", true)));
				}
			}
			if (oldTask.NumberHint != task.NumberHint)
			{
				var hint = task._hints.LastOrDefault();
				if (hint != null && !hint.IsEmpty())
					SndMsg(MessageToBot.GetTextMsg(new Texter($"{task.Alias}\n🔑Пришла подсказка:\n{hint.Name}\n{hint.Text}", true)));
			}
		
		}

		public void SendSectors(DzrPage page, bool update, bool all)
		{
			var task = GetMainTask(page);
			if (task == null)
				throw new GameException("Упс, не удалось получить результат");

			var result = new StringBuilder();

			result.Append(task.Alias);
			result.AppendLine();
			result.Append(!all ? "Остались:" : "Коды сложности:");
			result.AppendLine();
			foreach (var codes in task.Codes)
				result.AppendLine(codes.Text(!all, Environment.NewLine));

			//var msg = Controller.SendSectors(result.ToString(), all, update);

			//SndMsg(msg);
		}

		private void SendPageInfo(DzrPage page, bool onlyMain)
		{
			if (page == null)
			{
				
				SndMsg(MessageToBot.GetTextMsg("Не получить данные об игре"));
				return;
			}

			if (page.Type != PageType.GameGo)
			{
				SndMsg(MessageToBot.GetTextMsg(new Texter(page.SysMessage)));
		
				return;
			}
			
			if (onlyMain)
			{
				var task = GetMainTask(page);
				if (task != null)
				{
					var text = CheckChanges.GetTaskInfo(task, false, timeForEnd: page.TimeToEnd?.ToString());
					SndMsg(MessageToBot.GetTextMsg(text));
				}
			}
			else
				page.Tasks.ForEach(task => SndMsg(MessageToBot.GetTextMsg(CheckChanges.GetTaskInfo(task, false, null))));
		}

	
		private DzrTask GetMainTask(DzrPage page) => page?.Tasks?.Main(Settings.Game.Level);

		public static List<string> GetCodes(string str, string prefix)
		{
			//if (!Validator.Settings.Game.Send)
			//	return;

			if (string.IsNullOrEmpty(str))
				return null;

			if (str.StartsWith("."))
				return new List<string>() { str.Substring(1) };

			if (str.Contains(" "))
				return null;

			var msg = str.ToLower();

			var match = Regex.Match(msg, @"(\d|d|r|[^\s\w]|д|р|p)+");
			if (!match.Success)
				return null;

			if (match.Value != msg)
				return null;

			//ToDo: Через группы.
			var digMatch = Regex.Match(msg, @"\d*");
			if (digMatch.Success && digMatch.Value == msg)
				return new List<string>() { prefix + digMatch.Value, prefix + "r" + digMatch.Value, prefix + digMatch.Value + "r" };

			return new List<string>()
			{
				prefix + Regex.Replace(Regex.Replace(msg, @"д", "d"), @"р|p|[^\d\w]", "r")
			};
		}

		private void SendTextMsg(string message, IMessageId replaceMsgId = null, bool withHtml = false)
		{
			var t = MessageToBot.GetTextMsg(new Texter(message, withHtml));
			t.OnIdMessage = replaceMsgId;
			SndMsg(t);
		}

		/*protected void SndMsg(IList<IMessageToBot> messages)
		{
			SendMsgs?.Invoke(messages);
		}*/

		private void SndMsg(IMessageToBot messages)
		{
			SendMsg?.Invoke(messages);
		}
	}
}
