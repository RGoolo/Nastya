using System;
using System.Collections.Generic;
using Model;
using Model.Dummy;
using Model.Types.Class;
using Model.Types.Interfaces;
using Model.TelegramBot;
using Model.Logic.Model;
using Model.Types;
using System.Security;

namespace Nastya
{
	public class BotChatMaper
	{
		public Guid Bot { get; set; }
		public ChatMaper ChatMaper { get; set; }

		public BotChatMaper(Guid bot, ChatMaper chatMaper)
		{
			Bot = bot;
			ChatMaper = chatMaper;
		}
	}

	public class ManagerBots : IDisposable
	{
		private Dictionary<Guid, IBot> _bots;
		private Dictionary<Guid, BotChatMaper> _chats = new Dictionary<Guid, BotChatMaper>();

		bool cycle = false;
		Guid AdminBot;

		private Logger _loger = new Logger(nameof(ManagerBots));

		private SecureString GetBotToken()
		{
			var token = SecurityEnvironment.GetPassword("bot_token");
			if (token != null)
				return token;

			Console.WriteLine("Write telegram bot token:");
			token = new SecureString();

			var key = Console.ReadKey(true);
			while (key.Key != ConsoleKey.Enter)
			{
				token.AppendChar(key.KeyChar);
				key = Console.ReadKey(true);			
			}
			SecurityEnvironment.SetPassword(token, "bot_token");
			return token;
		}

		public ManagerBots()
		{
			AdminBot = Guid.NewGuid();
			var telegId = Guid.NewGuid();

			_bots = new Dictionary<Guid, IBot>
			{
				[AdminBot] = new DummyBot(AdminBot),
				[telegId] = new TelegramBot(GetBotToken(), telegId)
				{
					Id = telegId,
				},
			};
			FillBots();
		}

		private void FillBots()
		{
			_loger.WriteTrace(nameof(FillBots));
			foreach (var bot in _bots.Values)
			{
				var _source = new System.Threading.CancellationTokenSource();
				var _token = _source.Token;
				bot.StartAsync(_token);
			}
		}

		private void Bot_OnMessage(IMessage msg, Guid botId)
		{
			if (msg.Text == "/q" && AdminBot == botId)
				cycle = true;

			_loger.WriteTrace(nameof(Bot_OnMessage) + msg.Text);

			if (!_chats.ContainsKey(msg.ChatId))
			{
				var chatMapper = new ChatMaper(_bots[botId].TypeBot, msg.ChatId);
				_chats.Add(msg.ChatId, new BotChatMaper(botId, chatMapper));
			}
			try
			{
				var messages = _chats[msg.ChatId].ChatMaper.OnMessage(msg);
				messages.ForEach(x => x.ChatId = msg.ChatId);
				messages.ForEach(x => _bots[botId].SendMessage(x));
			}
			catch (MessageException mEx)
			{
				try
				{
					_loger.WriteError(mEx.StackTrace);
					var messsage = CommandMessage.GetErrorMsg(mEx);
					messsage.OnIdMessage = mEx.IMessage.MessageId;
					var tMsg = new TransactionCommandMessage(messsage)
					{ ChatId = msg.ChatId };
					_bots[botId].SendMessage(tMsg);
				}
				catch (Exception ex)
				{
					_loger.WriteError(ex.StackTrace);
				}
			}
			catch (ModelException mEx)
			{
				try
				{
					_loger.WriteError(mEx.StackTrace);
					var messsage = CommandMessage.GetErrorMsg(mEx);
					var tMsg = new TransactionCommandMessage(messsage)
					{ ChatId = msg.ChatId };
					_bots[botId].SendMessage(tMsg);
				}
				catch (Exception ex)
				{
					_loger.WriteError(ex.StackTrace);
				}

			}
			catch (Exception e)
			{
				try
				{
					_loger.WriteError(e.StackTrace);
					var messsage = CommandMessage.GetErrorMsg("Не удалось выполнить комманду");
					messsage.OnIdMessage = msg.MessageId;
					_bots[botId].SendMessage(new TransactionCommandMessage(messsage) { ChatId = msg.ChatId });
				}
				catch (Exception ex)
				{
					_loger.WriteError(ex.StackTrace);
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

		public void Wait()
		{
			while (!cycle)
			{
				foreach (var chat in _chats)
				{
					while (!chat.Value.ChatMaper.SendMessages.IsEmpty)
					{
						if (chat.Value.ChatMaper.SendMessages.TryDequeue(out var msg))
						{
							msg.ChatId = chat.Value.ChatMaper.ChatId;
							_bots[chat.Value.Bot].SendMessage(msg);
						}
					}
				}

				foreach (var bot in _bots)
				{
					IMessage msg;
					while ( ( msg = bot.Value.GetNewMessage() ) != null)
						Bot_OnMessage(msg, bot.Key);
					
				}
			}
			Dispose();
		}

		private void SendMsg(IEnumerable<MessageMarks> msgs)
		{
			if (msgs == null)
				return;
			foreach (var msg in msgs)
			{
				//_bots[msg.Message.BotId].SendMessage(msg, msg.Message.ChatId);
			}
		}

		private void SendMsg(IEnumerable<CommandMessage> msgs, IMessage imsg)
		{
			if (msgs == null)
				return;

			foreach (var msg in msgs)
			{
				//_bots[imsg.BotId].SendMessage(MessageMarks.GetMessageMarks(imsg, msg), imsg.ChatId);
			}
		}
	}
}

