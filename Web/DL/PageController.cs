using System;
using System.Text;
using Model.Bots.BotTypes.Class;
using Model.Bots.BotTypes.Enums;
using Model.Bots.BotTypes.Interfaces.Ids;
using Model.Logic.Model;
using Model.Logic.Settings;
using Web.DL.PageTypes;
using Web.Entitiy;

namespace Web.DL
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

		/*private void SendMsg(IList<IMessageToBot> msgs)
		{
			if (msgs == null) return;
			if (!msgs.Any()) return;
			_sendMsgDl.SendMsg2(msgs);
		}*/

		public void SendMsg(IMessageToBot msg)
		{
			_sendMsgDl.SendMsg2(msg);
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
					foreach (var msg in CheckChanges.Time(page, _lastPage))
						SendMsg(msg);

					foreach (var msg in CheckChanges.Hints(page, _lastPage))
						SendMsg(msg);

					foreach (var msg in CheckChanges.Sectors(page, _lastPage, _setting))
						SendMsg(msg);
				}
			}

			_lastPage = page;
			if (_lastPage.IsSturm != _setting.DlGame.Sturm)
				_setting.DlGame.Sturm = _lastPage.IsSturm;

			if (lvlNumber != _lastPage.LevelNumber)
				_setting.Page.LastLvl = _lastPage.LevelNumber;
		}

		public void SendNewLevelInfo(DLPage page, bool newLvl = false)
		{
			if (page == null) return;

			var message = MessageToBot.GetTextMsg(page.ToTexter(newLvl));
			
			if (newLvl)
			{
				message.Notification = Notification.NewLevel;
				message.NotificationObject = new[] { _lastPage, page };
			}

			SendMsg(message);
		}

		public void SendBonus(DLPage page, bool isAll = false)
		{
			StringBuilder sb = new StringBuilder("");

			if (page.Bonuses.IsEmpty)
				sb.Append("Нет бонусов");
			else
				sb.AppendLine(string.Join(Environment.NewLine,
					isAll ? page.Bonuses.AllBonuses : page.Bonuses.ReadyBonuses));
				
			var msg = MessageToBot.GetTextMsg(new Texter(sb.ToString() == "" ? "Все бонусы закрыты." : sb.ToString(), true));
			msg.Notification = Notification.SendSectors;
			SendMsg(msg);
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