using Model.Types.Class;
using System;
using System.Collections.Generic;
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
