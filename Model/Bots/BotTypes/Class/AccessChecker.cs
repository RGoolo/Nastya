using Model.Bots.BotTypes.Enums;

namespace Model.Bots.BotTypes.Class
{
	public static class AccessChecker
	{
		public static bool CheckAccess(TypeUser @class, TypeUser method, TypeUser user)
		{
			return CheckAccess(@class | TypeUser.Bot, user) && CheckAccess(method, user);
		}

		public static bool CheckAccess(TypeUser method, TypeUser user)
		{
			if (user.IsBot())
				return method.IsBot();

			if (user.IsAdmin() && user.IsDeveloper())
				return method.IsAdmin() || method.IsUser() || method.IsDeveloper();

			if (user.IsAdmin())
				return method.IsAdmin() || method.IsUser();

			if (user.IsDeveloper())
				return method.IsDeveloper();

			return user.IsUser() && method.IsUser();
		}
	}
}
