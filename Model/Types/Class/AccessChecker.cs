using Model.Types.Enums;

namespace Model.Types.Class
{
	public static class AccessChecker
	{
		public static bool CheckAccess(TypeUser @class, TypeUser method, TypeUser user)
		{
			return CheckAccess(@class, user) && CheckAccess(method, user);
		}

		public static bool CheckAccess(TypeUser method, TypeUser user)
		{
			return ContainsType(method, TypeUser.User) || (method & user) != 0;
		}

		public static bool ContainsType(TypeUser t, TypeUser t2)
		{
			return (t & t2) == t2;
		}
	}
}
