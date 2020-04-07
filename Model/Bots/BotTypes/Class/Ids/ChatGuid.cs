using System;
using Model.Bots.BotTypes.Interfaces.Ids;

namespace Model.Bots.BotTypes.Class.Ids
{
	public class ChatGuid : GuidId, IChatId
	{
		public ChatGuid(Guid value) : base(value)
		{
		}

		public ChatGuid(string value) : base(new Guid(value))
		{
		}

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

		public static bool operator ==(ChatGuid chat, IChatId chat2)
		{
			return chat?.GetId == chat2?.GetId;
		}

		public static bool operator !=(ChatGuid chat, IChatId chat2)
		{
			return !(chat == chat2);
		}
	}
}