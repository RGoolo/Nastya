using System;
using Model.BotTypes.Interfaces.Messages;

namespace Model.BotTypes.Enums
{
	[Flags]
	public enum TypeUser
	{
		User = 0x1,
		Developer = 0x2,
		Admin = 0x4,
	}

	public static class TypeUserExtension
	{
		public static bool IsAdmin(this TypeUser type) => (type & TypeUser.Admin) == TypeUser.Admin;
		public static bool IsDeveloper(this TypeUser type) => (type & TypeUser.Developer) == TypeUser.Developer;
	}
}