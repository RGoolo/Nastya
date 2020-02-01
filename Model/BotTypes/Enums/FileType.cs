using System;

namespace Model.BotTypes.Enums
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