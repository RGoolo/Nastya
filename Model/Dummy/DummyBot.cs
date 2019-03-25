using System;
using System.Threading;
using System.Threading.Tasks;
using Model.Types.Class;
using Model.Types.Enums;
using Model.Types.Interfaces;

namespace Model.Dummy
{
	public class DummyBot : BaseConcurrentBot
	{
		public DummyBot(Guid id) : base(TypeBot.Dummy, id)
		{
			
		}

		protected void Cycle2()
		{
			while (true)
			{
				if (!_toSendQueue.IsEmpty)
				{
					if (_toSendQueue.TryDequeue(out var tMsg))
					{
						if (tMsg.Message != null)
							Console.WriteLine(tMsg.Message.Texter);

						if (tMsg.Messages != null)
							foreach (var msg in tMsg.Messages)
								Console.WriteLine(msg.Texter);
					}
				}
				Thread.Sleep(100);
			}
		}

		public void Message(string text)
		{
			var msg = new Message(Id, null, text);
			_messagesQueue.Enqueue(msg);
		}

		public override void Dispose() => throw new NotImplementedException();

		protected override void RunCycle()
		{
			var text = Console.ReadLine();
			var msg = new Message(Id, null, text);
			EnqueueMessage(msg);
		}

		public override async Task<IMessage> Message(CommandMessage message, Guid chatId)
		{
			switch (message.TypeMessage)
			{
				case MessageType.Text:
					Console.WriteLine(message.Texter);
					break;
				case MessageType.Coordinates:
					Console.WriteLine($"{message.Coord} : {message.Texter}");
					break;
				case MessageType.Photo:
					Console.WriteLine($"{message.FileToken}");
					break;
			}
			return new Message(Id, null, nameof(Message));
		}

		protected override void DownloadFile(IMessage msg)
		{
			//empty
		}
	}
}
