using System;
using System.Collections.Generic;
using System.Linq;
using Model.Types.Class;

namespace Web.Game.Model
{
	public delegate void SendLightMsgDel(IEnumerable<CommandMessage> messages);

	public class Validator
	{
		protected Page _oldPage;
		//protected Settings _setting;

		public event SendLightMsgDel SendMsg;
		public string GetContextSetCode(string code) => $"LevelId={_oldPage?.LevelId ?? "1"}&LevelNumber={_oldPage?.LevelNumber ?? "1"}&LevelAction.Answer=" + code;

		private void SendTexttMsg(string message, Guid? replaceMsgId = null)
		{
			var msg = CommandMessage.GetTextMsg(message);
			msg.OnIdMessage = replaceMsgId.GetValueOrDefault();
		}

		private void SndMsg(IEnumerable<CommandMessage> messages)
		{
			SendMsg?.Invoke(messages);
		}

		private void SndMsg(CommandMessage message)
		{
			SndMsg(new List<CommandMessage>() { message, });
		}

		public virtual void AfterSendCode(Page page, string code, Guid idMsg)
		{
			if (!IsValid(page))
			{
				SendTexttMsg("Неизвестная ошибка, для кода: " + code, idMsg);
				return;
			}
			if (page?.IsReceived == null)
			{
				SendTexttMsg("Неизвестная ошибка, для кода: " + code, idMsg);
				return;
			}

			SendTexttMsg((page.IsReceived.Value ? "+," : "-,") + code, idMsg);

			SetNewPage(page);
		}

		public void SetNewPage(Page page)
		{
			if (NewLevel(page))
			{
				SendNewLevelInfo(page);
			}

			_oldPage = page;
		}

		protected bool IsValid(Page page) => page?.LevelNumber != null;

		protected bool NewLevel(Page page) => _oldPage != null && page.LevelNumber != _oldPage.LevelNumber;


		public void SendLevelInfo()
		{
			if (IsValid(_oldPage))
				SendNewLevelInfo(_oldPage);
			else
				SendTexttMsg("Нет данных");
		}

		public void SendNewLevelInfo(Page page)
		{
			var msg = new List<CommandMessage>
			{
				 CommandMessage.GetTextMsg(new Texter("❤️ Следующий уровень ❤️\n" + page.LevelTitle + "\n" + page.Task, true)),
			};

			msg.AddRange(page.ImageUrls.Select(x => CommandMessage.GetPhototMsg(x, new Texter(x))));

			SndMsg(msg);
		}
	}
}