using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Model.Bots.BotTypes;
using Model.Bots.BotTypes.Class;
using Model.Bots.BotTypes.Class.Ids;
using Model.Bots.BotTypes.Interfaces;
using Model.Bots.BotTypes.Interfaces.Ids;
using Model.Bots.BotTypes.Interfaces.Messages;
using Model.Logic.Model;
using Model.Logger;
using Nastya.Mappers;

namespace Nastya
{
	public class BotChatMapper
	{
		public IBotId Bot { get; set; }
		public ChatMapper ChatMapper { get; set; }

		public BotChatMapper(IBotId bot, ChatMapper chatMapper)
		{
			Bot = bot;
			ChatMapper = chatMapper;
		}
	}

	public class ManagerBots : IDisposable
	{
		private readonly Dictionary<IBotId, IBot> _bots;
		private readonly Dictionary<IChatId, BotChatMapper> _chats = new Dictionary<IChatId, BotChatMapper>();
		private readonly Dictionary<IChatId, IMessageCollection> _messages = new Dictionary<IChatId, IMessageCollection>();

		private readonly ILogger _logger = Logger.CreateLogger(nameof(ManagerBots));

		public ManagerBots(List<IBot> bots)
		{
            _bots = bots.ToDictionary(x => x.Id, x => x);
			FillBots();
		}

		private void FillBots()
		{
			_logger.Warning(nameof(FillBots));
			foreach (var bot in _bots.Values)
			{
				var source = new System.Threading.CancellationTokenSource();
				var token = source.Token;
				bot.StartAsync(token);
			}
		}

		private void LogAndSendException(IMessageToBot msg, Exception ex, IBotId botId, IChatId chatId, IMessageId msgId)
		{
			_logger.Error(ex);

			msg.OnIdMessage = msgId;
			var tMsg = new TransactionCommandMessage(msg);

			_bots[botId].SendMessage(chatId, tMsg);
		}

		private void Bot_OnMessage(IBotMessage msg, IBotId botId)
		{
			if (!_chats.TryGetValue(msg.Chat.Id, out var mapper))
			{
				var msgColl = new MessageCollection(msg.Chat.Id, botId);

				var chatMapper = new ChatMapper(_bots[botId].TypeBot, msg.Chat.Id, msgColl);
				_messages.Add(msg.Chat.Id, msgColl);
				mapper = new BotChatMapper(botId, chatMapper);
				_chats.Add(msg.Chat.Id, mapper);
			}
			try
			{
				var messages = mapper.ChatMapper.OnMessage(msg);

				foreach (var iCMsg in messages.SelectMany(x => x))
					iCMsg.OnIdMessage = msg.MessageId;

				_bots[botId].SendMessages(msg.Chat.Id, messages);
			}
			catch (MessageException mEx)
			{
				try
				{
					var message = MessageToBot.GetErrorMsg(mEx);
					LogAndSendException(message, mEx, botId, msg.Chat.Id, msg.MessageId);
				}
				catch (Exception ex)
				{
					_logger.Error(ex.StackTrace);
				}
			}
			catch (ModelException mEx)
			{
				try
				{
					var message = MessageToBot.GetErrorMsg(mEx);
					LogAndSendException(message, mEx, botId, msg.Chat.Id, msg.MessageId);
				}
				catch (Exception ex)
				{
					_logger.Error(ex);
				}
			}
			catch (Exception mEx)
			{
				try
				{
					var message = MessageToBot.GetErrorMsg("Не удалось выполнить комманду");
					LogAndSendException(message, mEx, botId, msg.Chat.Id, msg.MessageId);
				}
				catch (Exception ex)
				{
					_logger.Error(ex);
				}
			}	
		}

		public void Dispose()
		{
			foreach (var bot in _bots.Values)
			{
				//bot.OnMessage -= Bot_OnMessage;
				//bot.Dispose();
			}
		}

		private void SendToBotTask()
		{
			while (true)
			{
				foreach (var msgColl in _messages)
				{
					while (!msgColl.Value.IsEmpty)
					{
						if (msgColl.Value.TryGet(out var msg))
							_bots[msgColl.Value.BotId].SendMessage(msgColl.Value.ChatId, msg);
					}
				}

				foreach (var bot in _bots)
				{
					IBotMessage msg;
					while ((msg = bot.Value.GetNewMessage()) != null)
						Bot_OnMessage(msg, bot.Key);
				}
			}
		}

		public Task StartTask() => Task.Run(SendToBotTask);
    }
}

