using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Model.Logic.Settings;
using Web.Base;
using Web.Game.Model;
using Model.Logic.Model;
using Model.Types.Class;

namespace Web.DZR
{
	public class Validator : BaseValidator
	{
		private Page _lastPage;

		public Validator(ISettings setting) : base(setting)
		{

		}

		public override string GetContextSetSpoyler(string code) => GetMainTask(_lastPage)?.Spoilers.GetPostForCode(code);
		public override string GetContextSetCode(string code) => GetMainTask(_lastPage)?.GetPostForCode(code);

		public override void SetNewPage(string html) => SetNewPage(new Page(html, GetUrl()));
		public override string LogInContext() => $@"action=auth&login={Settings.Game.Login}&password={Settings.Game.Password}";

		public override string GetUrl()
		{
			if ((Settings.TypeGame & TypeGame.Dummy) == TypeGame.Dummy)
				return Settings.Web.Domen.Split('\\').SkipLast(1).Aggregate((x, y) => x + "\\" + y);

			return $@"http://{Settings.Web.Domen}/{Settings.Web.BodyRequest}/go/";
		}

		public override string LogInUrl()
		{
			string uri = GetUrl();
			return uri.Remove(uri.Length - 3);
		}

		public override void SendEvent(IEvent iEvent)
		{
			switch (iEvent.EventType)
			{
				case EventTypes.Refresh:
				case EventTypes.SendCode:
				case EventTypes.StartGame:
				case EventTypes.StopGame:
				case EventTypes.SendSpoiler:
					break;

				case EventTypes.GetLvlInfo:
					SendPageInfo(_lastPage, true);
					break;
				case EventTypes.GetBonus:
					break;
				case EventTypes.GetAllSectors:
					SendSectors(_lastPage, iEvent.Text);
					break;
				case EventTypes.GetSectors:
					SendSectors(_lastPage, iEvent.Text, true);
					break;
				case EventTypes.GetAllInfo:
					SendPageInfo(_lastPage, false);
					break;
				case EventTypes.GetTimeForEnd:
					SendTimeTiEnd();
					break;
			}
		}

		public void SendTimeTiEnd()
		{
			var task = GetMainTask(_lastPage);
			var time = _lastPage.TimeToEnd?.ToString("HH:mm:ss");
			var text = (task == null)? "⏳ Времени осталось: " + time : task.GetTextTimeToEnd(time);

			SendTexttMsg(text);
		}


		public override void AfterSendCode(string html, string code, Guid? idMsg)
		{
			var page = new Page(html, GetUrl());

			SendTexttMsg(page.GetAnswerText(code), idMsg);
			SetNewPage(page);
		}

		public void SetNewPage(Page page)
		{
			if (page == null || page.Type == PageType.NotFound)
				return;

			SendDifference(_lastPage, page);
			_lastPage = page;
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

		public void SendDifference(Page lastPage, Page newPage)
		{
			if (lastPage == null || lastPage.Type != newPage.Type)
			{
				SendPageInfo(newPage, !Settings.Game.CheckOtherTask);
					return;
			}

			SendDiffTime(lastPage.TimeToEnd, newPage.TimeToEnd);

			var checkOtherTask = Settings.Game.CheckOtherTask;
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

		private void SendDifference(Task task, Task oldTask)
		{
			if (task == null)
				return;

			var msg = new List<CommandMessage>();
			
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

		public void SendSectors(Page page, string start, bool all = false)
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

			var t = CommandMessage.GetTextMsg(result.ToString());
			//t.Notification = Model.Types.Enums.Notification. 
			SendTexttMsg(result.ToString(), withHtml:true);
		}

		public void SendPageInfo(Page page, bool onlyMain)
		{
			var msg = new List<CommandMessage>();

			if (page == null)
			{
				msg.Add(CommandMessage.GetTextMsg("Не получить данные об игре"));

				SndMsg(msg);
				return;
			}

			if (page.Type != PageType.GameGo)
			{
				msg.Add(CommandMessage.GetTextMsg(new Texter(page.SysMessage)));
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

		private List<CommandMessage> GetTaskInfo(Task task, bool newTask = false, string timeForEnd = null)
		{
			var msg = new List<CommandMessage>();
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

		private Task GetMainTask(Page page) => page?.Tasks?.Main(Settings.Game.Level);
	}
}
