using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Model.BotTypes.Class;
using Model.BotTypes.Enums;
using Model.BotTypes.Interfaces.Messages;
using Model.Logger;
using Model.TelegramBot;
using Telegram.Bot.Types;

namespace Model.BotTypes.Interfaces
{
	//public delegate void MessageArrivedDel(IMessage message);
	//public delegate void SendMessage(IMessageMark messageMark);

	public interface IConcurrentBot : IDisposable
	{
		List<IMessageToBot> ChildrenMessage(IMessageToBot msg, IChatId chatId);
		List<IBotMessage> GetMessages();
		Task<IBotMessage> Message( IMessageToBot message, IChatId chatId);
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
