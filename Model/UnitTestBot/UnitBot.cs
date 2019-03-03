using System;
using System.Threading;
using System.Threading.Tasks;
using Model.Types.Class;
using Model.Types.Enums;
using Model.Types.Interfaces;

namespace Model.UnitTestBot
{
	public class UnitTest : IBot
	{
		public Guid Id => throw new NotImplementedException();

		public TypeBot TypeBot => throw new NotImplementedException();

		public void Dispose()
		{
			throw new NotImplementedException();
		}

		public void DownloadResource(MessageMarks msg)
		{
			throw new NotImplementedException();
		}

		public void DownloadResource(IMessage msg)
		{
			throw new NotImplementedException();
		}

		public IMessage GetNewMessage()
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
