using System;
using Model.Bots.BotTypes.Interfaces.Ids;

namespace Model.Bots.BotTypes.Class.Ids
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