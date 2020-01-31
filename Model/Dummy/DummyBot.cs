using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Model.BotTypes.Class;
using Model.BotTypes.Enums;
using Model.BotTypes.Interfaces;
using Model.BotTypes.Interfaces.Messages;
using Model.Logger;

namespace Model.Dummy
{
	public class DummyBot : IConcurrentBot
	{
		public IBotId Id { get; }

		public ILogger Log { get; }

		public TypeBot TypeBot => TypeBot.Dummy;

		public DummyBot(IBotId id) 
		{
			Id = id;
			Log =  Logger.Logger.CreateLogger(id.ToString());
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

		public List<IBotMessage> GetMessages()
		{
			var text = Console.ReadLine();
			var msg = new Message(Id, null, text);
			return new List<IBotMessage> { msg };
		}

		public async Task<IBotMessage> Message(IMessageToBot message, IChatId chatId)
		{
			switch (message.TypeMessage)
			{
					case MessageType.SystemMessage:
					return null;
				case MessageType.Text:
					Console.WriteLine(message.Text);
					break;
				case MessageType.Coordinates:
					Console.WriteLine($"{message.Coordinate} : {message.Text}");
					break;
				case MessageType.Photo:
					Console.WriteLine($"{message.FileToken}");
					break;
			}
			
			return new Message(Id, message);
		}

		protected  void DownloadFile(IBotMessage msg)
		{
			//empty
		}

		

	
		public Task<IBotMessage> DownloadFileAsync(IBotMessage msg)
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
