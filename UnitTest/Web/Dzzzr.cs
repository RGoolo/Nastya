using System.Collections.Generic;
using Xunit;
using Model.Types.Enums;
using Model.Types.Class;
using Web.DZR;

namespace UnitTest.Model
{
	public class CheckRecognizeCode
	{
		
		[Theory]
		[MemberData(nameof(TypeUsers))]
		public void CoordTest(string text, string prefix, string[] result = null)
		{
		/*	var codes = Dzr.GetCodes(text, prefix) ?? new List<string>();

			if (codes == null && result == null)
				return;

			for(int i = 0; i < codes.Count; ++i)
				Assert.Equal(codes[i], result[i]);*/
		}

		public static IEnumerable<object[]> TypeUsers()
		{
			yield return new object[] { "abvd", "" };
			yield return new object[] { " d1", ""};
			yield return new object[] { "1", "1d", new[] { "1d1", "1dr1", "1d1r" } };
			yield return new object[] { "1a", "1d"};
			yield return new object[] { "1@", "1d", new[] { "1d1r" } };
			yield return new object[] { "r", "1d", new[] { "1dr" } };
			yield return new object[] { "pдd", "1d", new[] { "1drdd" } };
			yield return new object[] { "1)", "1d", new[] { "1d1r" } };
		}
	}
}
