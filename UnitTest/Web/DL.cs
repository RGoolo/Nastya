using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Model.BotTypes.Class;
using Model.BotTypes.Class.Ids;
using Model.BotTypes.Enums;
using Model.Logic.Settings;
using Model.TelegramBot;
using Web.Base;
using Web.DL;
using Web.DL.PageTypes;
using Xunit;

namespace UnitTest.Web
{
	public class DL
	{
		private const string nameBeforeStartedHtml = "Заглушка.html";
		private const string nameFirstLvlHtml = "первый уровень.html";
		private const string nameSecondLvlHtml = "куча бонусов.html";

		private class CollectionNotofication : ISendMsgDl
		{
			public readonly List<IMessageToBot> _messages = new List<IMessageToBot>();

			public void SendMsg(IEnumerable<IMessageToBot> msgs)
			{
				_messages.AddRange(msgs);
			}
		}

		//ToDo: path info
		private string GetFile(string file)
		{
			var bin = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent;
			var path = Path.Combine(bin.ToString(), "Web", "dlPages", file);
			using (StreamReader sr = new StreamReader(File.Open(path, FileMode.Open)))
				return sr.ReadToEnd();
		}



		[Fact]
		public void DLTest()
		{
			var testGuid =new ChatGuid( new Guid("C5121271-CF33-4EFA-B1E0-EE3486B1E724"));
			var notifications = new CollectionNotofication();
		
			var settings = SettingsHelper.GetSetting(testGuid);
			settings.Clear();

			var _pageController = new PageController(notifications, settings);
			var page = PageConstructor.GetNewPage(GetFile(nameBeforeStartedHtml));
			_pageController.SetNewPage(page);

			var page2 = PageConstructor.GetNewPage(GetFile(nameFirstLvlHtml));
			_pageController.SetNewPage(page2);

			var text = TelegramBot.GetNormalizeText(notifications._messages.First(x => x.Notification == Notification.NewLevel).Text, testGuid);
			
			Assert.Equal(page.Bonuses.Count, 0);
		}

		[Fact]
		public void DLTestHint()
		{
			var page = PageConstructor.GetNewPage(GetFile(nameSecondLvlHtml));

			Assert.False(page.Hints.IsEmpty);
		}

		[Fact]
		public void DlTestBorderValue()
		{
			var dt1 = TimeSpan.FromMinutes(5);
			var dt2 = TimeSpan.FromSeconds(255);
			Assert.True(BaseCheckChanges.IsBorderValue(dt1, dt2, 300));
		}

	}
}
