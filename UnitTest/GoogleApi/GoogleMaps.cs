using Model.Logic.Google;
using System.Collections.Generic;
using BotModel.Bots.BotTypes;
using BotModel.Bots.BotTypes.Class;
using BotModel.Bots.BotTypes.Class.Ids;
using BotModel.Settings;
using Model;
using Model.Settings;
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
			var password = SecurityEnvironment.GetTextPassword("google", "maps", "token");
			var settings = SettingsHelper.GetChatService(new ChatGuid(TestGuid));
			settings.Clear();
			
			var file = settings.FileChatFactory.NewResourcesFileByExt(".jpg");
			var factoryMaps = new GoogleImgForMaps(password);

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
