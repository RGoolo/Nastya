using System;

namespace Model.BotTypes.Class.Ids
{
	public class ChatGuid : GuidId, IChatId
	{
		public ChatGuid(Guid value) : base(value)
		{
		}

		public ChatGuid(string value) : base(new Guid(value))
		{
		}

	}
}