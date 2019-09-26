﻿using Model.Types.Class;
using Model.Types.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Model.Types.Interfaces
{
	//public delegate void MessageArrivedDel(IMessage message);
	//public delegate void SendMessage(IMessageMark messageMark);

	public interface IConcurentBot : IDisposable
	{
		List<IMessage> RunCycle();
		Task<IMessage> Message(CommandMessage message, Guid chatId);
		void PreCycle();
		Task<IMessage> DownloadFileAsync(IMessage msg);
		void OnError(Exception ex);

		Logger Log { get; }
		Guid Id { get; }
		TypeBot TypeBot { get; }
	}

	public interface IBot : IDisposable
	{
		Guid Id { get;}

		Task StartAsync(CancellationToken token);

		TypeBot TypeBot { get; }

		IMessage GetNewMessage();
		void SendMessage(TransactionCommandMessage tMessage);
		void DownloadResource(IMessage msg);
	}
}
