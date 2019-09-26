using Model.Types.Class;
using Model.Types.Interfaces;
using System;
using System.Collections.Generic;
using Web.Game.Model;

namespace Web.Base
{
	public delegate void SendMsgsSyncDel(IEnumerable<CommandMessage> messages);

	public interface ISendSyncMsgs
	{
		void SendSync(IEnumerable<CommandMessage> messages);
	}

	public interface IGame : IDisposable
	{
		void SetEvent(IEvent iEvent);
		void SendCode(string code, IUser user, Guid replaceMsg);
		void Start();
		void Stop();
	}
}
