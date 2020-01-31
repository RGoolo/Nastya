using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Model.BotTypes.Class;
using Model.BotTypes.Enums;
using Model.BotTypes.Interfaces;
using Model.BotTypes.Interfaces.Messages;

namespace Model.UnitTestBot
{
	public class UnitTest : IBot
	{
		public IBotId Id => throw new NotImplementedException();

		public TypeBot TypeBot => throw new NotImplementedException();

		public void Dispose()
		{
			throw new NotImplementedException();
		}

		public void SendMessages(IChatId chatId, List<TransactionCommandMessage> tMessage)
		{
			throw new NotImplementedException();
		}

		public void DownloadResource(IBotMessage msg)
		{
			throw new NotImplementedException();
		}

		public IBotMessage GetNewMessage()
		{
			throw new NotImplementedException();
		}

		public void SendMessage(IChatId chatId, TransactionCommandMessage tMessage)
		{
			throw new NotImplementedException();
		}

		public void SendMessage(TransactionCommandMessage tMessage)
		{
			throw new NotImplementedException();
		}

		public Task StartAsync(CancellationToken token)
		{
			throw new NotImplementedException();
		}
	}
}
