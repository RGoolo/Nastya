using System.Collections.Concurrent;
using Model.Bots.BotTypes.Class;
using Model.Bots.BotTypes.Interfaces.Ids;

namespace Nastya
{
	public interface ISendMessages
	{
		void Send(TransactionCommandMessage item);
	}

	public interface IMessageCollection : ISendMessages
	{
		IBotId BotId { get; }
		IChatId ChatId { get; }
		bool TryGet(out TransactionCommandMessage item);
		bool IsEmpty { get; }
	}

	public class MessageCollection : IMessageCollection
	{
		private ConcurrentQueue<TransactionCommandMessage> SendMessages = new ConcurrentQueue<TransactionCommandMessage>();

		public MessageCollection(IChatId chatId, IBotId botId)
		{
			ChatId = chatId;
			BotId = botId;
		}

		public void Send(TransactionCommandMessage item)
		{
			SendMessages.Enqueue(item);
		}


		public IBotId BotId { get; }
		public IChatId ChatId { get; }
		public bool TryGet(out TransactionCommandMessage msg)
		{
			return SendMessages.TryDequeue(out msg);
		}

		public bool IsEmpty => SendMessages.IsEmpty;
	}
}