using System;

namespace Model.BotTypes.Class.Ids
{
	public class BotGuid : GuidId, IBotId
	{
		public BotGuid(Guid value) : base(value)
		{
		}
		public BotGuid(string value) : base(new Guid(value))
		{
		}
	}
}