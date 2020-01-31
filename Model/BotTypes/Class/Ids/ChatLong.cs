using System;

namespace Model.BotTypes.Class.Ids
{
	public class ChatLong : ClassId<long>, IChatId
	{
		public ChatLong(long value) : base(value)
		{

		}

		public override Guid GetId => IdsMapper.ToGuid(Get);
	}
}