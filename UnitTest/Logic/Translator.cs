using Model.Types.Class;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xunit;

namespace UnitTest.Logic
{
	public class Translator
	{

		[Theory]
		[MemberData(nameof(Ints))]
		public void IntToGuid(int count)
		{
			var guid = count.ToGuid();
			Assert.Equal(count.ToString(), guid.ToInt().ToString());
		}

		public static IEnumerable<object[]> Ints()
		{
			yield return new object[] { int.MaxValue };
			yield return new object[] { int.MinValue };
			yield return new object[] { 0 };
		}

		[Theory]
		[MemberData(nameof(Longs))]
		public void LongToGuid(long count)
		{
			var guid = count.ToGuid();
			var lon = guid.ToLong();
			Assert.Equal(count.ToString(), lon.ToString());
		}

		public static IEnumerable<object[]> Longs()
		{
			yield return new object[] { long.MaxValue };
			yield return new object[] { long.MinValue };
		}
	}
}
