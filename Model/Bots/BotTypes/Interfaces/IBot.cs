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

	public interface IConcurrentBot<T> : IDisposable where T : IBotMessage
	{
        List<T> GetMessages();
		Task<T> SendMessage(IMessageToBot message, IChatId chatId);
		Task<T> DownloadFileAsync(IBotMessage msg);
		
		void OnError(Exception ex);

		ILogger Log { get; }
		IBotId Id { get; }
		TypeBot TypeBot { get; }
	}

	public interface IBot<T> : IDisposable where T : IBotMessage
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
