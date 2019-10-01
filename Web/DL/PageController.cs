using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model.Logic.Coordinates;
using Model.Logic.Model;
using Model.Logic.Settings;
using Model.Types.Class;
using Model.Types.Enums;
using Web.Base;
using Web.Game.Model;

namespace Web.DL.PageTypes
{
	public class PageController
	{
		private readonly ISendMsgDl _sendMsgDl;
		private DLPage _lastPage;
		private ISettings Setting;
		public const string TimeFormat = "HH:mm:ss";

		public void SendMsg(string message, Guid? replaceMsgId = null, bool withHtml = false)
		{
			var t = CommandMessage.GetTextMsg(new Texter(message, withHtml));
			t.OnIdMessage = replaceMsgId.GetValueOrDefault();
			SendMsg(t);
		}

		private void SendMsg(IEnumerable<CommandMessage> msgs)
		{
			_sendMsgDl.SendMsg(msgs);
		}

		public void SendMsg(CommandMessage msg)
		{
			var msgs = new List<CommandMessage>
			{
				msg,
			};
			SendMsg(msgs);
		}

		public PageController(ISendMsgDl sendMsgDl, ISettings settings)
		{
			_sendMsgDl = sendMsgDl;
			Setting = settings;
		}

		public void AfterSendCode(DLPage page, IEvent sendEvent)
		{
			var text = page.CodeType.ToText(sendEvent.Text);
			SendMsg(text, sendEvent.IdMsg);
			SetNewPage(page);
		}

		public void SetNewPage(DLPage page)
		{
			//if (Settings.GetValueBool(Game.))
			if (page == null) return;
			
			if (page.Type == TypePage.Unknown)
				throw new GameException("Сервер вернул некорректное значение.");

			if (page.Type == TypePage.Finished)
			{
				var msg = CommandMessage.GetTextMsg("Игра закончена.");
				msg.Notification = Notification.GameStoped;
				SendMsg(msg);
				return;
				//throw new GameException("Сервер вернул некорректное значение.");
			}

			var lvlNumber = Setting.Page.LastLvl;

			if (_lastPage == null)
			{
				_lastPage = page;
				if (lvlNumber != page.LevelNumber)
					SendNewLevelInfo(page, true);
			}
			else
			{
				if (_lastPage.LevelNumber != page.LevelNumber)
					SendNewLevelInfo(page, true);
				else
					SendDiffTime(page);
			}
			_lastPage = page;
			if (lvlNumber != _lastPage.LevelNumber)
				Setting.Page.LastLvl = _lastPage.LevelNumber;
		}

		private void SendDiffTime(DLPage page)
		{
			var msg = new List<CommandMessage>();

			if (IsBorderValue(page.TimeToEnd, _lastPage.TimeToEnd, 5))
				msg.Add(CommandMessage.GetTextMsg($"⏳ Осталось меньше 5 минут"));

			if (IsBorderValue(page.TimeToEnd, _lastPage.TimeToEnd, 1))
				msg.Add(CommandMessage.GetTextMsg($"⏳ Осталось меньше минуты"));

			for (var i = 0; i < page.Hints.Count; ++i)
			{
				if (!page.Hints[i].IsReady)
				{
					var hint = _lastPage.Hints.Where(x => x.Number == page.Hints[i].Number && page.Hints[i].Number != 0).FirstOrDefault();
					if (hint == null)
						continue;

					if (IsBorderValue(page.Hints[i].TimeToEnd, hint.TimeToEnd, 5))
						msg.Add(CommandMessage.GetTextMsg($"⏳ {hint.Name} Откроется через {hint.TimeToEnd.ToString(TimeFormat)}"));

					//if (IsBorderValue(page.Hints[i].TimeTo, hint.TimeTo, 1))
					//	msg.Add(CommandMessage.GetTextMsg($"⏳ {hint.Name} Откроется через {hint.TimeTo.ToString(TimeFormat)}"));
				}
			}
			if (msg.Any())
				SendMsg(msg);

		}

		public void SendNewLevelInfo(DLPage page, bool isNewlvl = false)
		{
			if (page == null) return;

			var msg = new List<CommandMessage>();

			StringBuilder sb = new StringBuilder();
			sb.AppendLine(!isNewlvl ? "📖 Текущий уровень" : $"📖 Новый уровень #{Setting.Web.GameNumber}");


			if (page.Levels.Any())
			{
				sb.Append("Уровни: \n");
				page.Levels.ForEach(x => sb.Append(x + "		"));
				sb.Append("\n");
			}

			if (page.TimeToEnd != default(DateTime))
				sb.Append($"⏳ Времени для автоперехода: " + page.TimeToEnd?.ToString(TimeFormat) + "\n");

			sb.Append(page.LevelTitle + "\n" + page.Task + "\n");
			/*if (page.Links.Count > 0)
			{
				sb.Append("В задании есть следующие ссылки: \n");
				page.Links.ForEach(x => sb.Append(x + "\n"));
			}*/

			if (!string.IsNullOrEmpty(page.Sectors?.SectorsRemain))
			{
				sb.Append($"На уровне осталось закрыть: {page.Sectors.SectorsRemain}(/sectors) из {page.Sectors.CountSectors}(/allsectors).\n");
			}

			if (page.Bonuses.Any())
			{
				var isReady = page.Bonuses.Count(x => x.IsReady);
				sb.Append($"на уровне закрыто {isReady}(/{Const.Game.Bonus}) из {page.Bonuses.Count()}(/{Const.Game.AllBonus})\n");
				//page.Bonuses.ForEach(x => sb.Append(x.IsReady + x.Name + "\n" + x.Text + "\n"));
				//sb.Append($"На уровне осталось закрыть: {page.Sectors.SectorsRemain}(/sectors) из {page.Sectors.CountSectors}(/allsectors).\n");
			}

			if (page.Hints.Any())
			{
				foreach (var hint in page.Hints)
				{
					sb.Append(hint.IsReady
						? $"\n{hint.Name}: {hint.Text}\n"
						: $"\n{hint.Name} откроется через: {hint.TimeToEnd.ToString(TimeFormat)}\n");
				}
			}

			var message = CommandMessage.GetTextMsg(new Texter(sb.ToString(), true));
			//msg.Add(message);
			
			if (isNewlvl)
			{
				//var messages = new SystemMess
				//
				message.Notification = Notification.NewLevel;
				message.NotificationObject = new DLPage[]{_lastPage, page};
			}

			msg.Add(message);

			/*
			 var textTask = WebHelper.RemoveImg(WebHelper.RemoteTagToTelegram(sb.ToString()));

			var text = textTask.Item1 + "\n";
			foreach (var img in textTask.Item2)
			{
				text = text.Replace(img.Name, $"<a href=\"{img.Url}\">{img.Name}</a>");
			}

			text = text.Replace("<a>", "</a>)");

			msg.Add(CommandMessage.GetTextMsg(new Texter(text, true)));

			var currentCoords = RegExPoints.GetCoords(sb.ToString()).ToList();
			foreach (var x in currentCoords)
				msg.Add(CommandMessage.GetCoordMsg(x));
								*/

			/*if (page.ImageUrls.Any())
			{
				if (page.ImageUrls.Count > 10)
					msg.Add(CommandMessage.GetTextMsg("Тут должны быть картинки, но их больше 10, так что не загружаю!"));
				else
					msg.AddRange(page.ImageUrls.Select(x => CommandMessage.GetPhototMsg(x, new Texter(x))));
			}*/
			SendMsg(msg);
		}

		private bool IsBorderValue(DateTime? dt1, DateTime? dt2, int minutes)
		{
			if (!dt1.HasValue || !dt2.HasValue)
				return false;

			DateTime maxDt;
			DateTime minDt;
			if (dt1 > dt2)
			{
				maxDt = dt1.Value;
				minDt = dt2.Value;
			}
			else
			{
				maxDt = dt2.Value;
				minDt = dt1.Value;
			}

			var difTime = new DateTime();
			difTime.AddMinutes(minutes);

			return ((maxDt - difTime).TotalMinutes > minutes && (minDt - difTime).TotalMinutes <= minutes);
		}

		public void SendBonus(DLPage page, bool isAll = false)
		{
			var msg = new List<CommandMessage>();
			StringBuilder sb = new StringBuilder("");

			if (!page.Bonuses.Any())
				sb.Append("Нет бонусов");
			else
				page.Bonuses.Where(x => isAll || !x.IsReady).ToList().ForEach(x => sb.Append(x.Name + ": " + x.Title + "\n" + (string.IsNullOrEmpty(x.Text) ? ("\n") : (x.Text + "\n\n"))));

			msg.Add(CommandMessage.GetTextMsg(sb.ToString() == "" ? "Все бонусы закрыты." : sb.ToString()));
			SendMsg(msg);
		}

		public void SendSectors(DLPage page, bool isAll = false)
		{
			StringBuilder sb = new StringBuilder();

			if (!string.IsNullOrEmpty(page.Sectors?.SectorsRemain))
			{
				var sectors = page.Sectors.Sectors.Where(x => (!x.Accepted || isAll));
				sb.Append($"На уровне осталось закрыть: {page.Sectors.SectorsRemain} из {page.Sectors.CountSectors}\n");
				sectors.ToList().ForEach(x =>
					sb.Append(
						$"{(x.Accepted ? "" : "<b>")}{x.Name} : {(x.Accepted ? x.Answer : "-")}{(x.Accepted ? "" : "</b>")}\n"));
			}
			else
			{
				sb.Append($"На уровне нет секторов");
			}

			var msg = CommandMessage.GetTextMsg(new Texter(sb.ToString(), true));
			msg.Notification = Notification.SendSectors;
			SendMsg(msg);
		}

		public DLPage GetCurrentPage => _lastPage;
	}
}