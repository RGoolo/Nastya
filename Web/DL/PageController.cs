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

		private void SendMsg(IReadOnlyCollection<IMessageToBot> msgs)
		{
			if (msgs == null) return;
			if (!msgs.Any()) return;
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
					SendMsg(CheckChanges.Time(page, _lastPage));
					SendMsg(CheckChanges.Hints(page, _lastPage));
					SendMsg(CheckChanges.Sectors(page, _lastPage, _setting));
				}
			}

			_lastPage = page;
			if (lvlNumber != _lastPage.LevelNumber)
				_setting.Page.LastLvl = _lastPage.LevelNumber;
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

			SendMsg(msg);
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

			if (!string.IsNullOrEmpty(page.Sectors?.SectorsRemainString))
			{
				var sectors = isAll ? page.Sectors.AllSectors : page.Sectors.AcceptedSectors;
				sb.Append(sectors);
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