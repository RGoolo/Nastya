using Model.Types.Class;
using System;
using System.Collections.Generic;
using System.Text;
using Web.Base;
using Web.DL;
using Web.DZR;
using Web.DZRLitePr;
using Web.Game.Model;

namespace Web.Base
{
	public delegate void SendMsgSyncDel(IEnumerable<CommandMessage> messages, Guid chatId);
	
	public interface IGame : IDisposable
	{ 
		event SendMsgSyncDel SendMsg;
		void SetEvent(IEvent iEvent);
		void SendCode(string code, Guid replaceMsg);
	}
}
