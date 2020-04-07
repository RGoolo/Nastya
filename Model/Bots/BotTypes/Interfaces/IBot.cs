using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Model.Bots.BotTypes.Class;
using Model.Bots.BotTypes.Enums;
using Model.Bots.BotTypes.Interfaces.Ids;
using Model.Bots.BotTypes.Interfaces.Messages;
using Model.Logger;

namespace Model.Bots.BotTypes.Interfaces
{
	//public delegate void MessageArrivedDel(IMessage message);
	//public delegate void SendMessage(IMessageMark messageMark);

	public interface IConcurrentBot : IDisposable
	{
		List<IMessageToBot> ChildrenMessage(IMessageToBot msg, IChatId chatId);
		List<IBotMessage> GetMessages();
		Task<IBotMessage> Message(IMessageToBot message, IChatId chatId);
		Task<IBotMessage> DownloadFileAsync(IBotMessage msg);
		
		void OnError(Exception ex);

		ILogger Log { get; }
		IBotId Id { get; }
		TypeBot TypeBot { get; }
	}

	public interface IBot : IDisposable
	{
		IBotId Id { get;}

		Task StartAsync(CancellationToken token);

		TypeBot TypeBot { get; }

		IBotMessage GetNewMessage();
		void SendMessage(IChatId chatId, TransactionCommandMessage tMessage);
		void SendMessages(IChatId chatId, List<TransactionCommandMessage> tMessage);
		void DownloadResource(IBotMessage msg);
	}
}
