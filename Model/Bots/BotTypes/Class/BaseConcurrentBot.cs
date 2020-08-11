using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Model.Bots.BotTypes.Enums;
using Model.Bots.BotTypes.Interfaces;
using Model.Bots.BotTypes.Interfaces.Ids;
using Model.Bots.BotTypes.Interfaces.Messages;
using Model.Logger;

namespace Model.Bots.BotTypes.Class
{
	public class ConcurrentBot<T> : IBot<T> where T : IBotMessage
	{
		private readonly ConcurrentQueue<(IChatId, TransactionCommandMessage)> _toSendQueue = new ConcurrentQueue<(IChatId, TransactionCommandMessage)>();
		private readonly ConcurrentQueue<IBotMessage> _downloadResourceQueue = new ConcurrentQueue<IBotMessage>();
		private readonly ConcurrentQueue<IBotMessage> _messagesQueue = new ConcurrentQueue<IBotMessage>();

		private readonly IConcurrentBot<T> _bot;


		public TypeBot TypeBot => _bot.TypeBot;
		public IBotId Id => _bot.Id;

		private readonly ILogger _log;
		//private object _lockerSendMsg = new object();

		public ConcurrentBot(IConcurrentBot<T> bot)
		{
			_bot = bot;
			_log = bot.Log; 
		}

		protected async Task Cycle()
		{
			_log.Warning(nameof(Cycle));
			
			while (true)
			{
				try
				{
					foreach(var msg in _bot.GetMessages())
						_messagesQueue.Enqueue(msg);
					await Messages();
					DownloadResources();
				}
				catch (System.Net.Http.HttpRequestException ex)
				{
					_log.Error(ex);
					//Thread.Sleep(1000);
				}
				catch (Exception ex)
				{
					//ToDo hey!
					try
					{
						_bot.OnError(ex);
					}
					catch
					{
						// ignored
					}

					_log.Error(ex.Message + ex.StackTrace);
					//Thread.Sleep(1000);
				}
			}
		}

		public IBotMessage GetNewMessage()
        {
            if (_messagesQueue.IsEmpty)
                return default;

			if (_messagesQueue.TryDequeue(out var msg))
				return msg;

			return default;
		}

		public void SendMessage(IChatId chatId, TransactionCommandMessage tMessage)
		{
			_toSendQueue.Enqueue((chatId, tMessage));
		}

		public void SendMessages(IChatId chatId, List<TransactionCommandMessage> tMessage) => tMessage.ForEach(m => SendMessage(chatId, m));

		public async Task StartAsync(CancellationToken token) => await Task.Run(Cycle, token);
		public void DownloadResource(IBotMessage msg) => _downloadResourceQueue.Enqueue(msg);
		protected void EnqueueMessage(IBotMessage msg) => _messagesQueue.Enqueue(msg);

		public void AfterSendMessage(IMessageToBot message, IBotMessage senderMsg)
		{
			if (message.SystemType == SystemType.NeedResource)
				DownloadResource((IBotMessage)message.SystemResource);
			// else if(senderMsg.TypeMessage == MessageType.SystemMessage)
			EnqueueMessage(senderMsg);
			
			/*switch (message.TypeMessage)
			{
				case Types.Enums.MessageType.SystemMessage:
					if (message.SystemType == SystemType.NeedResource)
						DownloadResource((IMessage)message.SystemResource);
					break;
				case Types.Enums.MessageType.Photo:
				case Types.Enums.MessageType.Voice:
					//EnqueueMessage()
					break;
			}*/
		}
		private async Task Messages()
		{
			if (_toSendQueue.IsEmpty)
				return;

			while (!_toSendQueue.IsEmpty)
			{
				if (!_toSendQueue.TryDequeue(out var msg)) continue;

				try
				{
					await MessagesSyncAsync(msg.Item1, msg.Item2);
				}
				catch (Exception e)
				{
					_toSendQueue.Enqueue( (msg.Item1, new TransactionCommandMessage(e.Message)));// ToDo retra
					_log.Error(e);
					throw;
				}
			}
		}

		private async Task MessagesSyncAsync(IChatId chatId, IEnumerable<IMessageToBot> tMessage)
		{
			if (tMessage == null) return;

			foreach (var msg in tMessage)
			{
				if (msg.SystemType == SystemType.NeedResource)
					 DownloadResource((IBotMessage)msg.SystemResource);
				else
				{
                    var botMsg = await _bot.SendMessage(msg, chatId);
					if (botMsg != null) AfterSendMessage(msg, botMsg);
                }
			}
		}

		protected async void DownloadResources()
		{
			while (!_downloadResourceQueue.IsEmpty)
			{
				if (_downloadResourceQueue.TryDequeue(out var msg))
				{
					if (msg == null)
						continue;

					var mesg = await _bot.DownloadFileAsync(msg);
					if (mesg != null)
						_messagesQueue.Enqueue(mesg);
				}
			}
		}

		public void Dispose()
		{
			_bot?.Dispose();
		}
	}
}
