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
					var msg = new List<CommandMessage>
					{

						new Text("Времени осталось: " + _lastPage.TimeToEnd?.ToString("HH:mm:ss")),
					};
					SndMsg(msg);
					break;
			}
		}

		public override void AfterSendCode(string html, string code, Guid? idMsg)
		{
			var page = new Page(html, GetUrl());
			SendTexttMsg(code + ". " + (page.SysMesssage ?? "Что-то непонятное"), idMsg);
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

			var time = new DateTime(0, 0, 0, 0, minutes, 0);
			if (lastTime > time && newTime < time)
				SendTexttMsg($"Осталось минут: {minutes}.");
		}

		private void SendDiffTime(DateTime? lastTime, DateTime? newTime)
		{
			SendDiffTime(lastTime, newTime, 5);
			SendDiffTime(lastTime, newTime, 1);

		}

		public void SendDifference(Page lastPage, Page newPage)
		{
			if (lastPage == null || lastPage.Type != newPage.Type)
			{
				SendPageInfo(newPage, !Settings.Game.CheckOtherTask);
					return;
			}

			if (newPage.Type == lastPage.Type && newPage.Type != PageType.GameGo)
			{
				SendDiffTime(lastPage.TimeToEnd, newPage.TimeToEnd);
					return;
			}

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
					{
						msg.Add(new Text($"{task.Alias}\nРазгадан спойлер:\n{task.Spoilers[i].Text}", true));
					}
				}
			}
			if (oldTask.NumberHint != task.NumberHint)
			{
				var hint = task._hints.LastOrDefault();
				if (hint != null && hint.Text != "\n---\n") 
					msg.Add(new Text($"{task.Alias}\nПришла подсказка:\n{hint.Name}\n{hint.Text}", true));
			}
			SndMsg(msg);
		}

		public void SendSectors(Page page, string start, bool all = false)
		{
			var task = GetMainTask(page);
			if (task == null)
				throw new GameException("Упс, не удалось получить результат");

			var result = new StringBuilder();

			if (!int.TryParse(start, result: out var i))
				i = 1;

			result.Append(task.Alias);
			result.Append(!all ? "\nОстались:" : "\nКоды сложности:");

			foreach (var codes in task.Codes)
			{
				result.Append("\n" + codes.Name + ":\n");
				foreach (var code in codes)
				{
					if (all || !code.Accepted)
						result.Append($"{i}) {code}\n");
					i++;
				}
				i = 1;
			}
			SendTexttMsg(result.ToString(), withHtml:true);
		}

		public void SendPageInfo(Page page, bool onlyMain)
		{
			var msg = new List<CommandMessage>();

			if (page == null)
			{
				msg.Add(new Text("Не получить данные об игре"));

				SndMsg(msg);
				return;
			}

			if (page.Type != PageType.GameGo)
			{
				msg.Add(new Text(page.SysMesssage));
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

			string split = "+++++";
			StringBuilder taskText = new StringBuilder();

			if (newTask)
				taskText.Append("!!!Новое Задание!!!\n");

			taskText.Append(task.TitleText + ".\n" );

			if (!string.IsNullOrEmpty(timeForEnd))
			{
				if (task._hints.Count == 0)
					taskText.Append($"Осталось времени до первой подсказки: {timeForEnd}");
				else if (task._hints.Count == 1)
					taskText.Append($"Осталось времени до второй подсказки: {timeForEnd}");
				else 
					taskText.Append($"Осталось времени до закрытия уровня: {timeForEnd}");
			}

			taskText.Append(task.Text + ".\n");

			if (task.Spoilers != null)
			{
				foreach (var spoiler in task.Spoilers)
				{
					taskText.Append(spoiler.IsComplited
						? $"{split}Спойлер разгадан:{split}\n{spoiler.Text}\n"
						: $"{split}Спойлер не разгадан!{split}\n");
				}
			}

			foreach (var hint in task._hints)
			{
				taskText.Append($"{split}{hint.Name}{split}\n{hint.Text}\n");
			}

			taskText.Append($"{split}Коды сложности /{Const.Game.Codes}:{split}");
			foreach (var codes in task.Codes)
			{
				taskText.Append("\n" + codes.Name + ":\n");
				int i = 0;
				foreach (var code in codes)
					taskText.Append($"{++i}:{code};");
			}
			msg.Add(new Text(taskText.ToString(), true, withHtml: true));
			msg.AddRange(task.Urls.Where(x => x.TypeUrl == WebHelper.TypeUrl.Img).Select(x => new Photo(x.Url, true, null, x.Name)));
			//msg.ForEach(x => x.Body = Web.Base.WebHelper.RemoveTag( x.Body));
			return msg;
		}

		private Task GetMainTask(Page page) => page?.Tasks?.Main(Settings.Game.Level);
	}
}
