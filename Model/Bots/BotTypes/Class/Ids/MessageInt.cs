using System;
using Model.Bots.BotTypes.Interfaces.Ids;

namespace Model.Bots.BotTypes.Class.Ids
{
	public class MessageInt : ClassId<int>, IMessageId
	{
		public MessageInt(int value, IChatId chatId) : base(value)
		{
			ChatId = chatId;
		}

		public override Guid GetId => IdsMapper.ToGuid(Get);
		public IChatId ChatId { get; }
	}

	public class MessageGuid : GuidId, IMessageId
	{
		public MessageGuid(Guid value, IChatId chatId) : base(value)
		{
			ChatId = chatId;
		}

		public IChatId ChatId { get; }
	}
}