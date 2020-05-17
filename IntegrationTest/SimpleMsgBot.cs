using System;
using System.Collections.Generic;
using System.Threading;
using Model.Bots.BotTypes.Class;
using Model.Bots.BotTypes.Class.Ids;
using Model.Bots.BotTypes.Interfaces;
using Model.Bots.UnitTestBot;
using Nastya;

namespace IntegrationTest
{
    public class SimpleMsgBot
    {
        private readonly UnitTestBot _testBot;

        public SimpleMsgBot()
        {
            var unitTestBot = new UnitTestBot(new BotGuid(new Guid()));
            new Services().StarBots(new List<IBot>() { unitTestBot });

            _testBot = unitTestBot;
        }
        
        //ToDo cancelationToken and Task.Delay
        public List<IMessageToBot> GetMessages(string str)
        {
            const int maxSecond = 1;

            var msg = new UnitTestMessage(str);
            _testBot.CreateNewMessage(msg);

            var dt = DateTime.Now;

            while ((DateTime.Now - dt).TotalSeconds < maxSecond)
            {
                var msgs = _testBot.GetMessageToBots(msg.Chat.Id);
                if (msgs != null)
                    return msgs;
                Thread.Sleep(10);
            }

            return null;
        }
    }
}