using System;

namespace Model.BotTypes.Enums
{
	[Flags]
	public enum TypeUser
	{
		User = 0x1,
		Developer = 0x2,
		Admin = 0x4,
	}
}