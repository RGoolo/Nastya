using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Model.Types.Class;
using Model.Types.Enums;
using Model.Types.Interfaces;

namespace Model.Dummy
{
	public class DummyBot : IConcurentBot
	{
		public Guid Id { get; }

		public Logger Log { get; }

		public TypeBot TypeBot => TypeBot.Dummy;

		public DummyBot(Guid id) 
		{
			Id = id;
			Log = new Logger(id.ToString());
		}

		/*protected void Cycle2()
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
		}*/

		public void Message(string text)
		{
			var msg = new Message(Id, null, text);
			//_messagesQueue.Enqueue(msg);
		}

		public  void Dispose() => throw new NotImplementedException();

		public List<IMessage> RunCycle()
		{
			var text = Console.ReadLine();
			var msg = new Message(Id, null, text);
			return new List<IMessage> { msg };
		}

		public async Task<IMessage> Message(CommandMessage message, Guid chatId)
		{
			
			switch (message.TypeMessage)
			{
					case MessageType.SystemMessage:
					return null;
				case MessageType.Text:
					Console.WriteLine(message.Texter);
					break;
				case MessageType.Coordinates:
					Console.WriteLine($"{message.Coordinate} : {message.Texter}");
					break;
				case MessageType.Photo:
					Console.WriteLine($"{message.FileToken}");
					break;
			}
			
			return new Message(Id, message);
		}

		protected  void DownloadFile(IMessage msg)
		{
			//empty
		}

		

		public void PreCycle()
		{
			//throw new NotImplementedException();
		}

		public Task<IMessage> DownloadFileAsync(IMessage msg)
		{
			return null;
			//throw new NotImplementedException();
		}

		public void OnError(Exception ex)
		{
			//throw new NotImplementedException();
		}
	}
}
