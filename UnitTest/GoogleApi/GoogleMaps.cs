using Model.Logic.Coordinates;
using Model.Logic.Google;
using Model.Logic.Settings;
using Model.Types.Class;
using Model.Types.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace UnitTest.GoogleApi
{
	public class GoogleMaps
	{
		// {C86B5F74-120A-4E8A-A888-BC768571DDFA}
		const string TestGuid = "{C86B5F74-120A-4E8A-A888-BC768571DDFA}";

		[Fact]
		public void Test()
		{
			var password = SecurityEnvironment.GetTextPassword("google_maps_token");
			var settings = SettingsHelper.GetSetting(new Guid(TestGuid));
			settings.Clear();
			
			var file = settings.FileWorker.NewFileTokenByExt(".jpg");
			var factoryMaps = new FactoryMaps(password, settings.FileWorker);

			var points = new List<Point>
			{
				new Coordinate(59.9225543f, 30.337371f, "Вокзал."),
				new Place("Площадь ленина 1"),
			};
			factoryMaps.SaveImg(file, points);
			Console.WriteLine(file.FileName);
		}

	}
}
