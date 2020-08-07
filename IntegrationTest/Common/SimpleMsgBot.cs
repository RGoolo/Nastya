using System;
using System.Collections.Generic;
using System.Threading;
using Model.Bots.BotTypes.Class;
using Model.Bots.BotTypes.Class.Ids;
using Model.Bots.BotTypes.Enums;
using Model.Bots.BotTypes.Interfaces;
using Model.Bots.BotTypes.Interfaces.Ids;
using Model.Bots.BotTypes.Interfaces.Messages;
using Model.Bots.UnitTestBot;
using Model.Files.FileTokens;
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
            var msg = new UnitTestMessage(str);
            return GetMessages(msg);
        }

        public List<IMessageToBot> GetMessages(string str, IChatId chatId, IResource resources)
        {
            var msg = new UnitTestMessage(str, chatId) {Resource = resources};
            return GetMessages(msg);
        }

        public List<IMessageToBot> GetMessages(string str, IChatId chatId)
        {
            var msg = new UnitTestMessage(str, chatId);
            return GetMessages(msg);
        }

        private List<IMessageToBot> GetMessages(IBotMessage msg)
        {
            const int maxSecond = 10;

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