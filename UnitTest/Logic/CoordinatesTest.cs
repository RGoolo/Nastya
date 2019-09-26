using System;
using System.Collections.Generic;
using System.Linq;
using Model.Logic.Coordinates;
using Model.Logic.Settings;
using Xunit;

namespace UnitTest.Logic
{
	public class CoordinatesTest
	{
		[Theory]
		[MemberData(nameof(OneCoordinate))]
		public void OneCoordinateTest(string coords, float latitude, float longtitude)
		{
			var coordinate = RegExPoints.GetCoords(coords).ToList();
			Assert.Single(coordinate);
			Assert.Equal(coordinate[0].Latitude, latitude);
			Assert.Equal(coordinate[0].Longitude, longtitude);
		}

		public static IEnumerable<object[]> OneCoordinate()
		{
	  //	  yield return new object[] { "55.755831S, 37.617673W — градусы", -55.755831f, -37.617673f };
	  //	  yield return new object[] { "55.755831N, 37.617673E — градусы", 55.755831f, 37.617673f };

			yield return new object[] { "N55°45'20\".992, E37°36'43\".200  — градусы(+ доп.буквы)", 55.7558327, 37.612 };
			yield return new object[] { "s55°45'20\".992, w37°36'43\".200  — градусы(+ доп.буквы)", -55.7558327, -37.612 };
			yield return new object[] { "s55°45'20.992\", w37°36'43.200\"  — градусы(+ доп.буквы)", -55.7558327, -37.612 }; 
			yield return new object[] { "55°45'20\".992N37°36'43\".200E — градусы и минуты(+ доп.буквы)", 55.7558327, 37.612 }; 
			yield return new object[] { "-55°45'20\".992″, -37°36'43\".200 — градусы, минуты и секунды(+ доп.буквы)", -55.7558327, -37.612 };
		}


		[Theory]
		[MemberData(nameof(CountCoordinate))]
		public void CountCoordinateTest(string coords, int count)
		{
			var coordinate = RegExPoints.GetCoords(coords);
		//	Assert.Equal(coordinate.Count,count);
		}
		public static IEnumerable<object[]> CountCoordinate()
		{

			yield return new object[] { "55.755831, 137.617673 — градусы", 0 }; //137
			//Good ToDo: add test
			yield return new object[] { "55.755831, 37.617673 — градусы", 1 };
			yield return new object[] { "55,755831, 37,617673 — градусы", 1 };
			yield return new object[] { "N55°75'5831, E55°75'5831  — градусы(+ доп.буквы)", 1 };
			yield return new object[] { "55°45.35′N, 37°37.06′E — градусы и минуты(+ доп.буквы)", 1 };
			yield return new object[] { "-55°45′20.9916″N, 37°37′3.6228″E — градусы, минуты и секунды(+ доп.буквы)", 1 };
			//Bad
			yield return new object[] { "155,755831, 37,617673 — градусы", 0} ; //155
			yield return new object[] { "55,755831, 137,617673 — градусы", 0 }; //137
			yield return new object[] { "-155°45′20.9916″N, 37°37′3.6228″E — градусы, минуты и секунды(+ доп.буквы)", 0 };
			yield return new object[] { "155°45′20.9916″N, 37°37′3.6228″E — градусы, минуты и секунды(+ доп.буквы)", 0 };
			yield return new object[] { "59.953519, 30.356323 59.933631, 30.305234 59.920544, 30.329020", 3 };
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