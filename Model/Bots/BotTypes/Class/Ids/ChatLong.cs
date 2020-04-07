using System;
using Model.Bots.BotTypes.Interfaces.Ids;

namespace Model.Bots.BotTypes.Class.Ids
{
	public class ChatLong : ClassId<long>, IChatId
	{
		public ChatLong(long value) : base(value)
		{
			GetId = IdsMapper.ToGuid(Get);
		}

		public override Guid GetId { get; }

		public override bool Equals(object obj)
		{
			if (obj is IChatId chatId)
				return chatId.GetId == GetId;
			return false;
		}

		protected bool Equals(ChatGuid other)
		{
			return other.GetId == GetId;
		}

		public override int GetHashCode()
		{
			return GetId.GetHashCode();
		}

		public static bool operator ==(ChatLong chat, IChatId chat2)
		{
			return chat?.GetId == chat2?.GetId;
		}

		public static bool operator !=(ChatLong chat, IChatId chat2)
		{
			return !(chat == chat2);
		}
	}
}