using System;

namespace Model.BotTypes.Class.Ids
{
	public class MessageInt : ClassId<int>, IMessageId
	{
		public MessageInt(int value) : base(value)
		{

		}

		public override Guid GetId => IdsMapper.ToGuid(Get);
	}

	public class MessageGuid : GuidId, IMessageId
	{
		public MessageGuid(Guid value) : base(value)
		{
		}
	}
}