using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.Bots.BotTypes.Class;
using Model.Bots.BotTypes.Class.Ids;
using Model.Bots.BotTypes.Interfaces;
using Model.Bots.UnitTestBot;
using Nastya;

namespace IntegrationTest
{
    [TestClass]
    public class ConcurrentBotTest
    {
        [TestMethod]
        public void TestMethod()
        {
            var unitTestBot = new UnitTestConcurrentBot(new BotGuid(new Guid()));
            var bot = new ConcurrentBot(unitTestBot);
            new Services().StarBots(new List<IBot>() { bot });

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