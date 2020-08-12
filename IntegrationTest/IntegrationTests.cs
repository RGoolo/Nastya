using System;
using BotModel.Bots.BotTypes.Class.Ids;
using BotModel.Bots.BotTypes.Enums;
using BotModel.Bots.BotTypes.Interfaces;
using BotModel.Bots.BotTypes.Interfaces.Messages;
using BotModel.Settings;
using IntegrationTest.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.Settings;

namespace IntegrationTest
{
    [TestClass]
    public class IntegrationTestsWithFiles
    {
        private readonly SimpleMsgBot _convert = new SimpleMsgBot();

        [TestMethod]
        public void TestMethod1()
        {
            var chatId = new ChatGuid(new Guid());
            
            var token = SettingsHelper<SettingHelper>.GetSetting(chatId).FileChatFactory.InternetFile("https://i.pinimg.com/236x/a9/d9/c0/a9d9c035ccfa8066af73cd59829097a7.jpg");
            IResource res = new Resource(token, TypeResource.Photo);
            
            var list = _convert.GetMessages("/ttt", chatId, res);

            Assert.IsNotNull(list);
            Assert.AreNotEqual(list.Count, 0);
        }
    }
}
