using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.Bots.BotTypes.Class;
using Model.Bots.BotTypes.Class.Ids;
using Model.Bots.BotTypes.Enums;
using Model.Bots.BotTypes.Interfaces.Messages;
using Model.Bots.UnitTestBot;
using Model.Logic.Settings;

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
            
            var token = SettingsHelper.GetSetting(chatId).FileChatFactory.InternetFile("https://i.pinimg.com/236x/a9/d9/c0/a9d9c035ccfa8066af73cd59829097a7.jpg");
            IResource res = new Model.Bots.BotTypes.Interfaces.Resource(token, TypeResource.Photo);
            
            var list = _convert.GetMessages("/ttt", chatId, res);

            Assert.IsNotNull(list);
            Assert.AreNotEqual(list.Count, 0);
        }
    }
}
