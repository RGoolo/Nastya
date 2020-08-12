using System.Collections.Generic;
using BotModel.Bots.BotTypes.Class;
using BotModel.Bots.BotTypes.Enums;
using Xunit;

namespace UnitTest.Model
{
	public class AccessCheckerTest
	{
		[Theory]
		[MemberData(nameof(TypeUsers))]
		public void CheckAccessTest(TypeUser method, TypeUser user, bool result)
		{
			Assert.Equal(AccessChecker.CheckAccess(method, user), result);
		}

		public static IEnumerable<object[]> TypeUsers()
		{
			yield return new object[] { TypeUser.Developer, TypeUser.Developer, true };
			yield return new object[] { TypeUser.Developer, TypeUser.Admin, false };
			yield return new object[] { TypeUser.Developer, TypeUser.User, false };

			yield return new object[] { TypeUser.Admin, TypeUser.Developer, false };
			yield return new object[] { TypeUser.Admin, TypeUser.Admin, true };
			yield return new object[] { TypeUser.Admin, TypeUser.User, false };

			yield return new object[] { TypeUser.User, TypeUser.Developer, false };
			yield return new object[] { TypeUser.User, TypeUser.Admin, true };
			yield return new object[] { TypeUser.User, TypeUser.User, true };

			yield return new object[] { TypeUser.Developer | TypeUser.Admin, TypeUser.Developer, true };
			yield return new object[] { TypeUser.Developer | TypeUser.Admin, TypeUser.Admin, true };
			yield return new object[] { TypeUser.Developer | TypeUser.Admin, TypeUser.User, false };

			yield return new object[] { TypeUser.User | TypeUser.Admin, TypeUser.Developer, false };
			yield return new object[] { TypeUser.User | TypeUser.Admin, TypeUser.Admin, true };
			yield return new object[] { TypeUser.User | TypeUser.Admin, TypeUser.User, true };

			yield return new object[] { TypeUser.User | TypeUser.Developer, TypeUser.User, true };
			yield return new object[] { TypeUser.User | TypeUser.Developer, TypeUser.User, true };
			yield return new object[] { TypeUser.User | TypeUser.Developer, TypeUser.User, true };

			yield return new object[] { TypeUser.User | TypeUser.Developer | TypeUser.Admin, TypeUser.Admin | TypeUser.Developer, true};
	
			yield return new object[] { TypeUser.User | TypeUser.Developer | TypeUser.Admin, TypeUser.Admin , true };
			yield return new object[] { TypeUser.User | TypeUser.Developer | TypeUser.Admin, TypeUser.Developer, true };
			yield return new object[] { TypeUser.User | TypeUser.Developer | TypeUser.Admin, TypeUser.User, true };

		}
	}
}