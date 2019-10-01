using Model.Types.Enums;
using Model.Types.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Model.Types.Class
{
	public class ConcurrentBot : IBot
	{
		private ConcurrentQueue<TransactionCommandMessage> _toSendQueue = new ConcurrentQueue<TransactionCommandMessage>();
		private ConcurrentQueue<IMessage> _downloadResourceQueue = new ConcurrentQueue<IMessage>();
		private ConcurrentQueue<IMessage> _messagesQueue = new ConcurrentQueue<IMessage>();

		private IConcurentBot _bot { get; }
		public TypeBot TypeBot => _bot.TypeBot;
		public Guid Id => _bot.Id;

		private readonly Logger _log;
		//private object _lockerSendMsg = new object();

		public ConcurrentBot(IConcurentBot bot)
		{
			_bot = bot;
			_log = bot.Log; 
		}

		protected async Task Cycle()
		{
			_log.WriteTrace(nameof(Cycle));
			try
			{
				_bot.PreCycle();
			}
			catch (Exception ex)
			{
				_log.WriteError(ex.Message + ex.StackTrace);
				return;
			}

			while (true)
			{
				try
				{
					foreach(var msg in _bot.RunCycle())
						_messagesQueue.Enqueue(msg);
					await Messages();
					DownloadResources();
				}
				catch (System.Net.Http.HttpRequestException ex)
				{
					_log.WriteError(ex.Message);
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

					_log.WriteError(ex.Message + ex.StackTrace);
					//Thread.Sleep(1000);
				}
			}
		}

		public IMessage GetNewMessage()
		{
			if (_messagesQueue.IsEmpty)
				return null;

			if (_messagesQueue.TryDequeue(out var msg))
				return msg;

			return null;
		}

		public void SendMessage(TransactionCommandMessage tMessage) => _toSendQueue.Enqueue(tMessage);
		public async Task StartAsync(CancellationToken token) => await Task.Run(() => Cycle(), token);
		public void DownloadResource(IMessage msg) => _downloadResourceQueue.Enqueue(msg);
		protected void EnqueueMessage(IMessage msg) => _messagesQueue.Enqueue(msg);

		public void AfterSendMessage(CommandMessage message, IMessage senderMsg)
		{
			if (message.SystemType == SystemType.NeedResource)
				DownloadResource((IMessage)message.SystemResource);
			else if(senderMsg.TypeMessage == MessageType.SystemMessage)
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
					await MessagesSyncAsync(msg);
				}
				catch (Exception e)
				{
					_log.WriteError("!!!!!!!!!!!!!!!!!");
					_log.WriteError(e.Message + Environment.NewLine + e.StackTrace);
					_toSendQueue.Enqueue(msg);
					_log.WriteError(e.Message);
					throw;
				}
			}
		}

		private async Task MessagesSyncAsync(TransactionCommandMessage tMessage)
		{
			foreach (var msg in tMessage)
			{
				var botMsg = await _bot.Message(msg, tMessage.ChatId);
				if (botMsg != null) _messagesQueue.Enqueue(botMsg);
			}
				
		}

		protected async void DownloadResources()
		{
			while (!_downloadResourceQueue.IsEmpty)
			{
				if (_downloadResourceQueue.TryDequeue(out var msg))
				{
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
