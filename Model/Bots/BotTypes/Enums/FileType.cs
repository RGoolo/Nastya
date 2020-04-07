using System;

namespace Model.Bots.BotTypes.Enums
{
	[Flags]
	public enum FileType
	{
		Internet,
		Local,

		Chat,

		Resources,
		Settings,

		CustomFile,
	}
}