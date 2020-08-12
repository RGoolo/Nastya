using System;
using System.Collections.Generic;
using System.Threading;
using BotModel.Bots.BotTypes.Class;
using BotModel.Bots.BotTypes.Class.Ids;
using BotModel.Bots.BotTypes.Interfaces;
using BotModel.Bots.BotTypes.Interfaces.Messages;
using BotModel.Bots.UnitTestBot;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NightGameBot;

namespace IntegrationTest
{
    [TestClass]
    public class ConcurrentBotTest
    {
        [TestMethod]
        public void TestMethod()
        {
            var unitTestBot = new UnitTestConcurrentBot(new BotGuid(new Guid()));
            var bot = (IBot)new ConcurrentBot(unitTestBot);
            new Services().StarBots(new List<IBot>() {bot });

            var list = Wait("/start", unitTestBot);
            Assert.IsNotNull(list);
            Assert.AreNotEqual(list.Count, 0);
        }

        public List<IMessageToBot> Wait(string str, UnitTestConcurrentBot unitTestBot)
        {
            const int maxSecond = 10;

            var msg = new UnitTestMessage(str);
            unitTestBot.CreateNewMessage(msg);

            var dt = DateTime.Now;

            while ((DateTime.Now - dt).TotalSeconds < maxSecond)
            {
                var msgs = unitTestBot.GetMessageToBots(msg.Chat.Id);
                if (msgs != null)
                    return msgs;
                Thread.Sleep(10);
            }

            return null;
        }
    }
}