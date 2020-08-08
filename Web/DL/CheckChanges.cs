using System;
using System.Collections.Generic;
using System.Linq;
using Model.Bots.BotTypes.Class;
using Model.Bots.BotTypes.Enums;
using Model.Logic.Settings;
using Web.Base;
using Web.DL.PageTypes;

namespace Web.DL
{
	public static class CheckChanges
	{
		public static List<IMessageToBot> Hints(DLPage page, DLPage lastPage)
		{
			if (page.Hints == null || page.Hints.IsEmpty)
				return null;

			if (lastPage.Hints == null || lastPage.Hints.IsEmpty)
				return null;

			var msg = new List<IMessageToBot>();
			foreach (var pageHint in page.Hints)
			{
				var lastHint = lastPage.Hints.GetById(pageHint.Number);
				if (lastHint == null)
					continue;

				if (!pageHint.IsReady || lastHint.IsReady) continue;

				var texter = new Texter(pageHint.ToString(), true);
				msg.Add(MessageToBot.GetTextMsg(texter));
			}

			return msg;
		}

		public static List<IMessageToBot> Time(DLPage page, DLPage lastPage)
		{
			var msg = new List<IMessageToBot>();

			if (BaseCheckChanges.IsBorderValue(page.TimeToEnd, lastPage.TimeToEnd, 300))
				msg.Add(MessageToBot.GetTextMsg("⏳ Осталось: " + page.TimeToEnd.Value.ToString("HH: mm:ss")));

			if (BaseCheckChanges.IsBorderValue(page.TimeToEnd, lastPage.TimeToEnd, 60))
				msg.Add(MessageToBot.GetTextMsg($"⏳ Осталось: " + page.TimeToEnd.Value.ToString("HH: mm:ss")));

			foreach (var newHint in page.Hints)
			{
				if (newHint.IsReady) continue;

				var lastHint = lastPage.Hints.FirstOrDefault(x => x.Number == newHint.Number && newHint.Number != 0);
				if (lastHint == null)
					continue;

				if (BaseCheckChanges.IsBorderValue(newHint.TimeToEnd, lastHint.TimeToEnd, 300))
					msg.Add(MessageToBot.GetTextMsg(newHint.ToString()));

				if (BaseCheckChanges.IsBorderValue(newHint.TimeToEnd, lastHint.TimeToEnd, 60))
					msg.Add(MessageToBot.GetTextMsg(newHint.ToString()));
			}

			return msg;
		}

		public static List<IMessageToBot> Sectors (DLPage page, DLPage lastPage, ISettings settings)
		{
			var msgs = new List<IMessageToBot>();
			// public static List<IMessageToBot> Hints(DLPage page, DLPage lastPage)
			if (page.Sectors.AcceptedSectors.Count != lastPage.Sectors.AcceptedSectors.Count)
			{
				var msg = MessageToBot.GetEditMsg(new Texter(page.Sectors.ToString(false), true));
				msg.EditMsg = settings.Game.SectorsMsg;
				msg.Notification = Notification.SendSectors;
				msgs.Add(msg);

				var msgAll = MessageToBot.GetEditMsg(new Texter(page.Sectors.ToString(true), true));
				msgAll.EditMsg = settings.Game.AllSectorsMsg;
				msg.Notification = Notification.SendAllSectors;
				msgs.Add(msgAll);
			}

			return msgs;
		}
	}
}