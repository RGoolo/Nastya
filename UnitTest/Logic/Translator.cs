using System.Collections.Generic;
using BotModel.Bots.BotTypes.Class;
using Xunit;

namespace UnitTest.Logic
{
	public class Translator
	{

		[Theory]
		[MemberData(nameof(Ints))]
		public void IntToGuid(int count)
		{
			var guid = IdsMapper.ToGuid(count);
			Assert.Equal(count.ToString(), IdsMapper.ToInt(guid).ToString());
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
			var guid = IdsMapper.ToGuid(count);
			var lon = IdsMapper.ToLong(guid);
			Assert.Equal(count.ToString(), lon.ToString());
		}

		public static IEnumerable<object[]> Longs()
		{
			yield return new object[] { long.MaxValue };
			yield return new object[] { long.MinValue };
		}
	}
}
