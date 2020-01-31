using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model.BotTypes.Class;
using Model.BotTypes.Enums;
using Model.Logic.Model;
using Model.Logic.Settings;
using Web.Base;
using Web.Game.Model;

namespace Web.DL.PageTypes
{
	public class PageController
	{
		private readonly ISendMsgDl _sendMsgDl;
		private DLPage _lastPage;
		private readonly ISettings _setting;
		

		public void SendMsg(string message, IMessageId replaceMsgId = null, bool withHtml = false)
		{
			var t = MessageToBot.GetTextMsg(new Texter(message, withHtml));
			t.OnIdMessage = replaceMsgId;
			SendMsg(t);
		}

		private void SendMsg(IEnumerable<IMessageToBot> msgs)
		{
			_sendMsgDl.SendMsg(msgs);
		}

		public void SendMsg(IMessageToBot msg)
		{
			var msgs = new List<IMessageToBot>
			{
				msg,
			};
			SendMsg(msgs);
		}

		public PageController(ISendMsgDl sendMsgDl, ISettings settings)
		{
			_sendMsgDl = sendMsgDl;
			_setting = settings;
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
				var msg = MessageToBot.GetTextMsg("Поздравляю, вы закончили!");
				msg.Notification = Notification.GameStoped;
				SendMsg(msg);
				return;
			}

			var lvlNumber = _setting.Page.LastLvl;

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
				{
					SendDiffTime(page);
					SendDiffHint(page);
				}
			}
			_lastPage = page;
			if (lvlNumber != _lastPage.LevelNumber)
				_setting.Page.LastLvl = _lastPage.LevelNumber;
		}

		private void SendDiffHint(DLPage page)
		{
			if(page.Hints == null || page.Hints.IsEmpty)
				return;

			if (_lastPage.Hints == null || _lastPage.Hints.IsEmpty)
				return;

			var msg = new List<IMessageToBot>();
			foreach (var pageHint in page.Hints)
			{
				var lastHint = _lastPage.Hints.GetById(pageHint.Number);
				if (lastHint == null)
					continue;

				if (!pageHint.IsReady || lastHint.IsReady) continue;

				var texter = new Texter(pageHint.ToString(), true);
				msg.Add(MessageToBot.GetTextMsg(texter));
			}
			if (msg.Any())
				SendMsg(msg);
		}

		private void SendDiffTime(DLPage page)
		{
			var msg = new List<IMessageToBot>();

			if (IsBorderValue(page.TimeToEnd, _lastPage.TimeToEnd, 300))
				msg.Add(MessageToBot.GetTextMsg($"⏳ Осталось меньше 5 минут"));

			if (IsBorderValue(page.TimeToEnd, _lastPage.TimeToEnd, 60))
				msg.Add(MessageToBot.GetTextMsg($"⏳ Осталось меньше минуты"));

			foreach (var newHint in page.Hints)
			{
				if (newHint.IsReady) continue;

				var lastHint = _lastPage.Hints.FirstOrDefault(x => x.Number == newHint.Number && newHint.Number != 0);
				if (lastHint == null)
					continue;

				if (IsBorderValue(newHint.TimeToEnd, lastHint.TimeToEnd, 300))
					msg.Add(MessageToBot.GetTextMsg(newHint.ToString()));

				if (IsBorderValue(newHint.TimeToEnd, lastHint.TimeToEnd, 60))
					msg.Add(MessageToBot.GetTextMsg(newHint.ToString()));
			}
			if (msg.Any())
				SendMsg(msg);

		}

		public void SendNewLevelInfo(DLPage page, bool newLvl = false)
		{
			if (page == null) return;

			var msg = new List<IMessageToBot>();
			
			var message = MessageToBot.GetTextMsg(page.ToTexter(newLvl, _setting.DlGame.TimeFormat));
			
			if (newLvl)
			{
				message.Notification = Notification.NewLevel;
				message.NotificationObject = new[] { _lastPage, page };
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

		private bool IsBorderValue(DateTime? dt1, DateTime? dt2, int second)
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
			difTime.AddSeconds(second);

			return ((maxDt - difTime).TotalSeconds > second && (minDt - difTime).TotalSeconds <= second);
		}

		public void SendBonus(DLPage page, bool isAll = false)
		{
			var msgs = new List<IMessageToBot>();
			StringBuilder sb = new StringBuilder("");

			if (page.Bonuses.IsEmpty)
				sb.Append("Нет бонусов");
			else
				sb.AppendLine(string.Join(Environment.NewLine,
					isAll ? page.Bonuses.AllBonuses : page.Bonuses.ReadyBonuses));
				
			var msg = MessageToBot.GetTextMsg(new Texter(sb.ToString() == "" ? "Все бонусы закрыты." : WebHelper.ReplaceAudioLinks(sb.ToString()), true));
			msg.Notification = Notification.SendSectors;
			msgs.Add(msg);

			var souds = WebHelper.GetAudioLinks(sb.ToString());

			foreach (var sound in souds)
			{
				var vMsg = MessageToBot.GetVoiceMsg(sound.Url, sound.Name);//
				vMsg.Notification = Notification.Sound;
				msgs.Add(vMsg);
			}
			SendMsg(msgs);
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

			var msg = MessageToBot.GetTextMsg(new Texter(sb.ToString(), true));
			msg.Notification = Notification.SendSectors;
			SendMsg(msg);
		}

		public DLPage GetCurrentPage => _lastPage;
	}
}