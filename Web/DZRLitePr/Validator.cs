using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Web.Base;
using Web.Game.Model;
using Model.Logic.Model;
using Model.Logic.Settings;
using Model.Types.Class;
using static System.Int32;
using Model.Types.Interfaces;
using Model.Types.Enums;

namespace Web.DZRLitePr
{
	public class Validator : BaseValidator
	{
		private string lastPage;
		public Validator(ISettings setting) : base(setting)
		{

		}

		private Page _lastPage;


		public override void AfterSendCode(string html, IUser user, string code, Guid? idMsg)
		{
			Page page = null;
			page = (Settings.TypeGame & TypeGame.Dummy) == TypeGame.Dummy ? new Page(html, string.Empty) : new Page(html, GetUrl());
		
			SendTexttMsg(page.AnswerText ?? "Что-то непонятное", idMsg);
			SetNewPage(page);
		}

		public void SetNewPage(Page page)
		{
			if (_lastPage == null)
			{
				_lastPage = page;
				SendPageInfo(_lastPage);
				return;
			}
			SendDifference(_lastPage, page);
			_lastPage = page;
		}

		public void SendDifference(Page lastPage, Page newPage)
		{
			

			foreach (var task in newPage._tasks)
			{
				var msgs = new List<CommandMessage>();
				var oldTask = lastPage._tasks.FirstOrDefault(x => x.LvlNumber == task.LvlNumber);

				if (oldTask == null)
				{
					msgs.AddRange(GetTaskInfo(task, true));
					continue;
				}

				if (oldTask.Spoiler != null && task.Spoiler != null)
				{
					if (!oldTask.Spoiler.IsComplited && task.Spoiler.IsComplited)
					{
						var msg = CommandMessage.GetTextMsg(new Texter($"{task.Alias}\nРазгадан спойлер:\n{task.Spoiler.Text}", true));
						msg.Notification = Notification.NewSpoiler;
						msgs.Add(msg);
					}
						
				}

				if (oldTask.NumberHint != task.NumberHint)
				{
					var hint = task._hints.LastOrDefault();
					if (hint != null)
					{
						var msg = CommandMessage.GetTextMsg(new Texter($"{task.Alias}\nПришла подсказка:\n{hint.Name}\n{hint.Text}", true));
						msg.Notification = Model.Types.Enums.Notification.NewHint;
						msgs.Add(msg);
					}
				}

				SndMsg(msgs);
			}
		}

		public override void SetNewPage(string html)
		{
			string uri = string.Empty;
			if ((Settings.TypeGame & TypeGame.Dummy) != TypeGame.Dummy)
				uri = GetUrl();

			SetNewPage(new Page(html, uri));
		}


		public override string GetContextSetCode(string code) => GetMainTask(_lastPage)?.GetPostForCode(code);


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
					SendSectors(_lastPage, iEvent.Text, true);
					break;
				case EventTypes.GetSectors:
					SendSectors(_lastPage, iEvent.Text);
					break;
				case EventTypes.GetAllInfo:
					SendPageInfo(_lastPage);
					break;
			}
		}

		public void SendSectors(Page page, string start, bool all = false)
		{
			var task = GetMainTask(page);
			if (task == null)
				throw new GameException("Упс, не удалось получить результат");

			var result = new StringBuilder();

			if (!TryParse(start, out var i))
				i = 1;


			result.Append(task.Alias);
			result.Append(!all ? "\nОстались:" : "\nКоды сложности:");

			foreach (var codes in task.Codes)
			{
				result.Append("\n" + codes.Name + ":\n");
				foreach (var code in codes.ListCode)
				{
					if (all || !code.Accepted)
						result.Append($"{i}{code}");
					i++;
				}
				i = 1;
			}
			SendTexttMsg(result.ToString());
		}

		public void SendPageInfo(Page page, bool onlyMain = false)
		{
			var msg = new List<CommandMessage>();
			if (onlyMain)
			{
				var task = GetMainTask(page);
				if (task != null)
					msg.AddRange(GetTaskInfo(task));
			}
			else
				page._tasks.ForEach(task => msg.AddRange(GetTaskInfo(task)));

			SndMsg(msg);
		}

		private List<CommandMessage> GetTaskInfo(Task task, bool newTask = false)
		{
			var msg = new List<CommandMessage>();

			string split = "-------------------------------";
			StringBuilder taskText = new StringBuilder();

			if (newTask)
				taskText.Append("!!!Новое Задание!!!\n");

			taskText.Append(task.TitleText + ".\n");
			taskText.Append(task.Text + ".\n");

			if (task.Spoiler != null)
			{
				taskText.Append(task.Spoiler.IsComplited
					? $"{split}\nСпойлер разгадан:\n{task.Spoiler.Text}\n"
					: $"{split}\nСпойлер не разгадан!\n");
			}

			foreach (var hint in task._hints)
			{
				taskText.Append($"{split}\n{hint.Name}\n{hint.Text}\n");
			}

			taskText.Append($"{split}\nКоды сложности:");
			foreach (var codes in task.Codes)
			{
				taskText.Append("\n" + codes.Name + ":\n");
				int i = 0;
				foreach (var code in codes.ListCode)
					taskText.Append($"{++i}{code}");
			}
			msg.Add(CommandMessage.GetTextMsg(new Texter(taskText.ToString(), true)));
			//msg.AddRange(task.Urls.Where(x => x.TypeUrl == WebHelper.TypeUrl.Img).Select(x => new Photo(x.Url, true, null, x.Name)));
			return msg;
		}

		private Task GetMainTask(Page page)
		{
			if (page == null)
				return null;
			var task = page._tasks.FirstOrDefault(x => x.LvlNumber == Settings.Game.Level);
			return task ?? (page._tasks.Any() ? page._tasks[0] : null);
		}


		public override string LogInContext() => $@"action=auth&login={Settings.Game.Login}&password={Settings.Game.Password}";

		public override string LogInUrl() => GetUrl();

		public override string GetUrl() => $@"http://{Settings.Web.Domen}/{Settings.Web.BodyRequest}/";
	}
}