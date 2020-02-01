using System;
using System.Collections.Generic;
using System.Security;
using Model.BotTypes.Class;
using Model.BotTypes.Class.Ids;
using Model.BotTypes.Interfaces;
using Model.Dummy;
using Model.TelegramBot;

namespace Nastya
{
	public static class BotsFactory
	{
		public static List<IBot> Bots(IBotId adminGuid)
		{
			var bots = new List<IBot>
			{
				new ConcurrentBot(new DummyBot(adminGuid)),
				new ConcurrentBot(new TelegramBot(GetBotToken(), new BotGuid(Guid.NewGuid()))),
			};
			
			return bots;
		}

		private static SecureString GetBotToken()
		{
			var token = SecurityEnvironment.GetPassword("telegram_bot_token");
			if (token != null)
			{
				//Console.WriteLine("Use old telegram bot token?");
				//if (string.Equals(Console.ReadLine(), "y", StringComparison.OrdinalIgnoreCase))
					return token;
			}

			Console.WriteLine("Write a telegram bot token:");
			token = new SecureString();

			var key = Console.ReadKey(true);
			while (key.Key != ConsoleKey.Enter)
			{
				token.AppendChar(key.KeyChar);
				key = Console.ReadKey(true);
			}

			SecurityEnvironment.SetPassword(token, "telegram_bot_token");
			return token;
		}
	}
}