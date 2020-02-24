using System;
using Model.BotTypes.Interfaces.Messages;

namespace Model.BotTypes.Enums
{
	[Flags]
	public enum TypeUser
	{
		None = 0,
		User = 1,
		Developer = 1 << 1,
		Admin = 1 << 2,
		Bot = 1 << 3,
		All = ~0,
	}

	public static class TypeUserExtension
	{
		public static bool IsAdmin(this TypeUser type) => (type & TypeUser.Admin) == TypeUser.Admin;
		public static bool IsUser(this TypeUser type) => (type & TypeUser.User) == TypeUser.User;
		public static bool IsBot(this TypeUser type) => (type & TypeUser.Bot) == TypeUser.Bot;
		public static bool IsDeveloper(this TypeUser type) => (type & TypeUser.Developer) == TypeUser.Developer;
	}
}