using Model.Logic.Coordinates;
using Model.Logic.Google;
using Model.Logic.Settings;
using System;
using System.Collections.Generic;
using Model.BotTypes.Class;
using Model.BotTypes.Class.Ids;
using Xunit;
using Xunit.Abstractions;

namespace UnitTest.GoogleApi
{
	public class GoogleMaps
	{
		private readonly ITestOutputHelper _testOutputHelper;

		public GoogleMaps(ITestOutputHelper testOutputHelper)
		{
			_testOutputHelper = testOutputHelper;
		}

		// {C86B5F74-120A-4E8A-A888-BC768571DDFA}
		const string TestGuid = "{C86B5F74-120A-4E8A-A888-BC768571DDFA}";

		[Fact]
		public void Test()
		{
			var password = SecurityEnvironment.GetTextPassword("google_maps_token");
			var settings = SettingsHelper.GetSetting(new ChatGuid(TestGuid));
			settings.Clear();
			
			var file = settings.FileChatWorker.NewResourcesFileTokenByExt(".jpg");
			var factoryMaps = new FactoryMaps(password);

			var points = new List<Point>
			{
				new Coordinate(59.9225543f, 30.337371f, "Вокзал."),
				new Place("Площадь ленина 1"),
			};
			factoryMaps.SaveImg(file, points);
			_testOutputHelper.WriteLine(file.FileName);
		}

	}
}
