using System;
using Google.Cloud.Speech.V1;
using Model.Logic.Google;
using Model.Logic.Settings;
using Model.Types.Class;
using Xunit;

namespace UnitTest.GoogleApi
{
	public class Recognition
	{
		const string TestGuid = "{C86B5F74-120A-4E8A-A888-BC768571DDFA}";

		[Fact]
		public void TestVision()
		{ 
			var settings = SettingsHelper.GetSetting(new Guid(TestGuid));
			settings.Clear();
			var file = settings.FileWorker.GetExistFileByPath(@"C:\Users\Roman\source\repos\RGoolo\Nastya\UnitTest\sources\test.png");

			var text = Vision.GetTextAsync(settings.FileWorker, file).Result;
			Console.WriteLine(text);
			
			//"C:\Users\Roman\source\repos\RGoolo\Nastya\UnitTest\GoogleApi\test.png"


		}

		[Fact]
		public void TestSpeech()
		{
			var settings = SettingsHelper.GetSetting(new Guid(TestGuid));
			settings.Clear();
			var file = settings.FileWorker.GetExistFileByPath(@"C:\Users\Roman\source\repos\RGoolo\Nastya\UnitTest\sources\test.ogg");

			var text = Voice.GetText(settings.FileWorker, file).Result;
			Console.WriteLine(text); //тест распознавания речи

			Assert.Equal(text.Trim().ToLower(), "тест распознавания речи");
			//"C:\Users\Roman\source\repos\RGoolo\Nastya\UnitTest\GoogleApi\test.png"


		}
		
		[Fact]
		public void TestRecogniseCode()
		{
			var settings = SettingsHelper.GetSetting(new Guid(TestGuid));
			settings.Clear();
			var file = settings.FileWorker.GetExistFileByPath(@"C:\Users\Roman\source\repos\RGoolo\Nastya\UnitTest\sources\testSendCode.ogg");

			var text = Voice.GetText(settings.FileWorker, file).Result;
			Console.WriteLine(text); //тест распознавания речи

			//Assert.Equal(text.Trim().ToLower(), "тест распознавания речи");
			//"C:\Users\Roman\source\repos\RGoolo\Nastya\UnitTest\GoogleApi\test.png"


		}
	}
}