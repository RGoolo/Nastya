using System;
using System.Collections.Generic;
using Model.Bots.BotTypes.Class;
using Model.Bots.BotTypes.Class.Ids;
using Model.Logic.Coordinates;
using Model.Logic.Settings;
using Model.Logic.Settings.Classes;
using Xunit;

namespace UnitTest.Logic
{
	
	public class CoordinatesProviderTest
	{
		[Theory]
		[MemberData(nameof(OneCoordinate))]
		public void OneCoordinateTest(string coords)
		{
			var set = SettingsHelper.GetSetting(new ChatGuid(Guid.Empty));

			set.Coordinates.GoogleCreads = SecurityEnvironment.GetTextPassword("google", "maps", "token");
			var factory = set.PointsFactory;
			var point = factory.GetCoordinates(coords);
			var str = point.ReplacePoints();
		}

		[Theory]
		[MemberData(nameof(OneCoordinate))]
		public void CoordinateTest(string coords)
		{
			var set = SettingsHelper.GetSetting(new ChatGuid(Guid.Empty));

			var factory = set.PointsFactory;
			var point = factory.GetCoordinates(coords);
			var str = point.TotalPoints();
		}

		public static IEnumerable<object[]> OneCoordinate()
		{
			yield return new object[] { "N55°45'20\".992, E37°36'43\".200  — градусы(+ доп.буквы)\nN55°45'20\".992, E37°36'43\".200  — градусы(+ доп.буквы)" };
		}
		public static IEnumerable<object[]> OnePlace()
		{
			yield return new object[] { "Невский проспект, 85;Санкт-Петербург (Финляндский вокзал);Витебский вокзал" };
		}

		[Theory]
		[MemberData(nameof(OnePlace))]
		public void OnePlaceTest(string coords)
		{
			var set = SettingsHelper.GetSetting(new ChatGuid(Guid.Empty));
			var factory = set.PointsFactory;
			var point = factory.GetPlaces(coords);
			var str = point.ReplacePoints();
			var str2 = str;
		}
	}
}