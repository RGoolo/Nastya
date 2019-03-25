using Model.Types.Enums;
using Model.Types.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Model.Types.Class
{
	public abstract class BaseConcurrentBot : IBot
	{
		protected ConcurrentQueue<TransactionCommandMessage> _toSendQueue = new ConcurrentQueue<TransactionCommandMessage>();
		protected ConcurrentQueue<IMessage> _downloadResourceQueue = new ConcurrentQueue<IMessage>();
		protected ConcurrentQueue<IMessage> _messagesQueue = new ConcurrentQueue<IMessage>();

		public TypeBot TypeBot { get; }
		public Guid Id { get; set; }
		public abstract void Dispose();

		protected readonly Logger _loger;
		private object _lockerSendMsg = new object();

		protected BaseConcurrentBot(TypeBot typeBot, Guid id)
		{
			Id = id;
			TypeBot = typeBot;
			_loger = new Logger($"{id} {typeBot}");
		}

		protected abstract void RunCycle();
		public abstract Task<IMessage> Message(CommandMessage message, Guid chatId);
		protected virtual void PreCycle() { /*empty*/ }
		protected abstract void DownloadFile(IMessage msg);
		protected virtual void OnError(Exception ex)
		{

		}

		protected void Cycle()
		{
			_loger.WriteTrace(nameof(Cycle));
			try
			{
				PreCycle();
			}
			catch (Exception ex)
			{
				_loger.WriteError(ex.Message + ex.StackTrace);
				return;
			}

			while (true)
			{
				try
				{
					RunCycle();
					Messages();
					DownloadResources();
				}
				catch (System.Net.Http.HttpRequestException ex)
				{
					_loger.WriteError(ex.Message);
					Thread.Sleep(1000);
				}
				catch (Exception ex)
				{
					//ToDo hey!
					try
					{
						OnError(ex);
					}
					catch { }

					_loger.WriteError(ex.Message + ex.StackTrace);
					Thread.Sleep(1000);
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
			switch (message.SystemType)
			{
				case SystemType.NeedResource:
					DownloadResource((IMessage)message.SystemResource);
					break;
				case SystemType.None:
					break;
				default:
					EnqueueMessage(senderMsg);
					break;
			}

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
					_toSendQueue.Enqueue(msg);
					_loger.WriteError(e.Message);
					throw;
				}
			}
		}

		private async Task MessagesSyncAsync(TransactionCommandMessage tMessage)
		{
			if (tMessage.Message != null)
				//lock (_lockerSendMsg) I am lucky!
			{
				var a = await Message(tMessage.Message, tMessage.ChatId);
				AfterSendMessage(tMessage.Message, a);
			}

			if (tMessage.Messages != null)
			{
				foreach (var msg in tMessage.Messages.ToList())
					//lock (_lockerSendMsg) I am lucky!
				{
					var b = Message(msg, tMessage.ChatId);
					tMessage.Messages.Remove(msg);
					AfterSendMessage(msg, b.Result);
				}
			}
		
		}

		protected void DownloadResources()
		{
			while (!_downloadResourceQueue.IsEmpty)
				if (_downloadResourceQueue.TryDequeue(out var msg))
					DownloadFile(msg);
		}
	}
}
