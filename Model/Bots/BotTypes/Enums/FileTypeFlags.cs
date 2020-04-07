using System;

namespace Model.Bots.BotTypes.Enums
{
	[Flags]
	public enum FileTypeFlags
	{
		Undefined = 0,
		
		IsLocal = 1 << 1,
		IsChat = 1 << 2,
		
		IsCustomFile = 1 << 3,

		Resources = 1 << 4,
		Settings = 1 << 5,
	}
}