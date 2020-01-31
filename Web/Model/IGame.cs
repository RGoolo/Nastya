using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Model.BotTypes.Class;
using Model.BotTypes.Interfaces;
using Model.BotTypes.Interfaces.Messages;
using Web.Game.Model;

namespace Web.Base
{
	public delegate void SendMsgsSyncDel(IEnumerable<IMessageToBot> messages);

	public interface ISendSyncMsgs
	{
		void SendSync(IEnumerable<IMessageToBot> messages);
	}

	public interface IGame : IDisposable
	{
		void SetEvent(IEvent iEvent);
		void SendCode(string code, IUser user, IMessageId replaceMsg);
		Task Start();
		void Stop();
	}
}
