using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Model.BotTypes.Class;
using Model.BotTypes.Interfaces;
using Model.BotTypes.Interfaces.Messages;
using Model.Logic.Settings;
using Web.Base;
using Web.Game.Model;
using Model.Logic.Model;

namespace Web.DZR
{
	public class DzrController : IController
	{
		private DzrPage _lastPage;
		private DzrWebValidator _dzrWebValidator;


		public DzrController(ISettings settings)
		{
			Settings = settings;
			_dzrWebValidator = new DzrWebValidator(settings);
		}


		public event SendMsgsSyncDel SendMsgs;
		public ISettings Settings { get; }
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
					var page = _dzrWebValidator.SendCode(iEvent.Text, GetMainTask(_lastPage)).Result; //ToDo delete result
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
					SendTimeTiEnd();
					break;
			}
		}

		public void Refresh()
		{
			var page = _dzrWebValidator.GetNextPage().Result; //ToDo delete result
			SetNewPage(page);
		}

		public void SendTimeTiEnd()
		{
			var task = GetMainTask(_lastPage);
			var time = _lastPage?.TimeToEnd?.ToString("HH:mm:ss");
			var text = (task == null)? "⏳ Времени осталось: " + time : task.GetTextTimeToEnd(time);

			SendTexttMsg(text);
		}


	/*	public override void AfterSendCode(string html, IUser user, string code, Guid? idMsg)
		{
			var page = new DzrPage(html, GetUrl());

			//Controller(page, )
			//var t = CommandMessage.GetTextMsg(new Texter(message, withHtml));
			//t.OnIdMessage = replaceMsgId.GetValueOrDefault();

			//Controller.SetGameAnswer( )

			SendTexttMsg(page.GetAnswerText(code), idMsg);
			SetNewPage(page);
			SendSectors(page, false, false);
			SendSectors(page, false, true);
		}*/

		public void SetNewPage(DzrPage page)
		{
			if (page == null || page.Type == PageType.NotFound)
				return;

			SendDifference(_lastPage, page);
			_lastPage = page;
			
			var lvlName = GetMainTask(_lastPage)?.LvlNumber;
			//Controller.SetNewLvl(lvlName);
		}

		private void SendDiffTime(DateTime? lastTime, DateTime? newTime , int minutes)
		{
			if (lastTime == null || newTime == null)
				return;

			var time = new DateTime().AddMinutes(minutes);

			if (lastTime.Value > time && newTime.Value <= time)
				SendTimeTiEnd();
		}

		private void SendDiffTime(DateTime? lastTime, DateTime? newTime)
		{
			//for (var i = 0; i < 15; ++i)
			SendDiffTime(lastTime, newTime, 1);
			SendDiffTime(lastTime, newTime, 5);
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
				SendDifference(GetMainTask(newPage), GetMainTask(lastPage));
			}
			else
			{
				foreach (var task in newPage.Tasks)
				{
					var oldTask = lastPage.Tasks?.FirstOrDefault(x => x.LvlNumber == task.LvlNumber);
					SendDifference(task, oldTask);
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

					SendTexttMsg($"⚠️Новые коды:\n{newTask.Codes[i].DiffText(diffs)}");

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


		private void SendDifference(DzrTask task, DzrTask oldTask)
		{
			if (task == null)
				return;

			var msg = new List<IMessageToBot>();
			
			if (oldTask == null || task?.LvlNumber != oldTask?.LvlNumber)
			{
				msg.AddRange(GetTaskInfo(task, true));
				SndMsg(msg);
				return;
			}

			if (task.Spoilers?.Count > 0)
			{
				for (var i = 0; i < task.Spoilers.Count(); ++i)
				{
					if (!(i < oldTask.Spoilers?.Count)) continue;

					if (!oldTask.Spoilers[i].IsComplited && task.Spoilers[i].IsComplited)
						msg.AddRange(WebHelper.ReplaceTextOnPhoto($"{task.Alias}\n🔑Разгадан спойлер:\n{task.Spoilers[i].Text}", task.DefaulUri));
				}
			}
			if (oldTask.NumberHint != task.NumberHint)
			{
				var hint = task._hints.LastOrDefault();
				if (hint != null && !hint.IsEmpty()) 
					msg.AddRange(WebHelper.ReplaceTextOnPhoto($"{task.Alias}\n🔑Пришла подсказка:\n{hint.Name}\n{hint.Text}", task.DefaulUri));
			}
			SndMsg(msg);
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

		public void SendPageInfo(DzrPage page, bool onlyMain)
		{
			var msg = new List<IMessageToBot>();

			if (page == null)
			{
				msg.Add(MessageToBot.GetTextMsg("Не получить данные об игре"));

				SndMsg(msg);
				return;
			}

			if (page.Type != PageType.GameGo)
			{
				msg.Add(MessageToBot.GetTextMsg(new Texter(page.SysMessage)));
				SndMsg(msg);
				return;
			}
			
			if (onlyMain)
			{
				var task = GetMainTask(page);
				if (task != null)
					msg.AddRange(GetTaskInfo(task, timeForEnd: page.TimeToEnd?.ToString("HH:mm:ss")));
			}
			else
				page.Tasks.ForEach(task => msg.AddRange(GetTaskInfo(task)));

			SndMsg(msg);
		}

		private List<IMessageToBot> GetTaskInfo(DzrTask task, bool newTask = false, string timeForEnd = null)
		{
			var msg = new List<IMessageToBot>();
			StringBuilder taskText = new StringBuilder();

			if (newTask)
				taskText.Append("📩Новое Задание\n");

			taskText.Append(task.TitleText + Environment.NewLine );

			if (!string.IsNullOrEmpty(timeForEnd))
				taskText.Append(timeForEnd);

			taskText.Append(task.Text + Environment.NewLine);

			if (task.Spoilers != null)
				taskText.Append(task.Spoilers.Text());
			
			foreach (var hint in task._hints)
				taskText.Append($"📖{hint.Name}\n{hint.Text}{Environment.NewLine}");
			
			taskText.Append($"Коды сложности /{Const.Game.Codes} остались /{Const.Game.LastCodes}:\n");

			foreach (var codes in task.Codes)
				taskText.AppendLine(codes.Text());

			return WebHelper.ReplaceTextOnPhoto(taskText.ToString(), task.DefaulUri);
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

		protected void SendTexttMsg(string message, IMessageId replaceMsgId = null, bool withHtml = false)
		{
			var t = MessageToBot.GetTextMsg(new Texter(message, withHtml));
			t.OnIdMessage = replaceMsgId;
			SndMsg(t);
		}

		protected void SndMsg(IEnumerable<IMessageToBot> messages)
		{
			SendMsgs?.Invoke(messages);
		}

		protected void SndMsg(IMessageToBot messages)
		{
			var msg = new List<IMessageToBot>
			{
				messages,
			};
			SendMsgs?.Invoke(msg);
		}

	}
}
