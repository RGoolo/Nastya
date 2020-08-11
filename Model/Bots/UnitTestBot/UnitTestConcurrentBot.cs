using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Model.Bots.BotTypes.Class;
using Model.Bots.BotTypes.Enums;
using Model.Bots.BotTypes.Interfaces;
using Model.Bots.BotTypes.Interfaces.Ids;
using Model.Bots.BotTypes.Interfaces.Messages;
using Model.Logger;

namespace Model.Bots.UnitTestBot
{
	public class UnitTestConcurrentBot : IConcurrentBot<UnitTestMessage>
	{
        //ToDo concurrent
        private readonly Queue<UnitTestMessage> _messages = new Queue<UnitTestMessage>();
        //ToDo concurrent
        private readonly Dictionary<IChatId, List<IMessageToBot>> _messageToBots = new Dictionary<IChatId, List<IMessageToBot>>();

        public List<IMessageToBot> ChildrenMessage(IMessageToBot msg, IChatId chatId)
        {
           return new List<IMessageToBot>();
        }

        public List<UnitTestMessage> GetMessages()
        {
            var list = new List<UnitTestMessage>();
            while (_messages.TryDequeue(out var msg))
                list.Add(msg);

            return list;
        }

        public Task<UnitTestMessage> SendMessage(IMessageToBot message, IChatId chatId)
        {
            if (!_messageToBots.TryGetValue(chatId, out var list))
            {
                list = new List<IMessageToBot>();
                _messageToBots.Add(chatId, list);
            }

            list.Add(message);

            return Task.Run(MessageRun);
            //toDo return new  UnitTestMessage(senderMsg, true, message);
        }

        private UnitTestMessage MessageRun()
        {
            return null;
        }

        public Task<UnitTestMessage> DownloadFileAsync(IBotMessage msg)
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception ex)
        {
           
        }

        public ILogger Log { get; } = Logger.Logger.CreateLogger(nameof(UnitTestConcurrentBot));
        public IBotId Id { get; }

        public TypeBot TypeBot => TypeBot.UnitTest;

        public UnitTestConcurrentBot(IBotId id)
        {
            Id = id;
        }


        public void Dispose()
        {
            
        }

        public void CreateNewMessage(UnitTestMessage msg)
        {
            _messages.Enqueue(msg);
        }

        public List<IMessageToBot> GetMessageToBots(IChatId chatId)
        {
            if (_messageToBots.TryGetValue(chatId, out var list))
                return list;
            return null;
        }
    }
}
