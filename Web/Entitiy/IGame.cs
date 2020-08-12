using System;
using System.Threading.Tasks;
using BotModel.Bots.BotTypes.Class;
using BotModel.Bots.BotTypes.Interfaces.Ids;
using BotModel.Bots.BotTypes.Interfaces.Messages;

namespace Web.Entitiy
{
	// public delegate void SendMsgsSyncDel(IList<IMessageToBot> messages);
	public delegate void SendMsgDel(IMessageToBot message);

	public interface ISenderSyncMsgs
	{
		IChatId ChatId { get; }
		// void SendSync(IList<IMessageToBot> messages);
		void Send(IMessageToBot messages);
	}

	public interface IGameControl : IGame, IControl
	{

	}

	public interface IControl : IDisposable
	{ 
		Task Start();
		void Stop();
	}

	public interface IGame
	{
		Guid GameId { get; }

		void SetEvent(IEvent iEvent);
		void SendCode(string code, IUser user, IMessageId replaceMsg);
	}
}
