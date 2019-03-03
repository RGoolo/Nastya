using System;
using System.Collections.Generic;
using Xunit;

namespace UnitTest.Logic
{
	public class CoordinatesTest
	{
		[Theory]
		[MemberData(nameof(StartCoordinates))]
		public void CoordTest(string coord, int count)
		{
			//Assert.Equal(count, ISettingsCoordinates.GetCoords(coord).Count());
		}

		public static IEnumerable<object[]> StartCoordinates()
		{
			//Good ToDo: add test
			yield return new object[] { "55.755831, 37.617673 — градусы", 1 };
			//yield return new object[] { "55,755831, 37,617673 — градусы", 1 };
			//yield return new object[] { "N55°75'5831, E55°75'5831  — градусы(+ доп.буквы)", 1 };
			//yield return new object[] { "55°45.35′N, 37°37.06′E — градусы и минуты(+ доп.буквы)", 1 };
			//yield return new object[] { "-55°45′20.9916″N, 37°37′3.6228″E — градусы, минуты и секунды(+ доп.буквы)", 1 };
			//Bad
			yield return new object[] { "155,755831, 37,617673 — градусы", 0} ; //155
			yield return new object[] { "55,755831, 137,617673 — градусы", 0 }; //137
		}

		[Fact]
		public void LoadTesting()
		{
			var dt = DateTime.Now;
		//	for (int i = 0; i < 10000; i++)
		//		ISettingsCoordinates.GetCoords($"55.755831, 37.617673 — градусы{i}").ToList();

			Assert.True((DateTime.Now - dt).TotalSeconds < 1);
		}

	}
}