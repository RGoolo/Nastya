using System;
using System.Collections.Generic;
using System.Security;
using Model.Bots.BotTypes.Class;
using Model.Bots.BotTypes.Class.Ids;
using Model.Bots.BotTypes.Interfaces;
using Model.Bots.BotTypes.Interfaces.Ids;
using Model.Bots.CmdBot;
using Model.Bots.TelegramBot.Entity;
using Model.Bots.UnitTestBot;

namespace Nastya
{
	public static class BotsFactory
	{
		public static List<IBot> Bots()
		{
			var bots = new List<IBot>
			{
				new ConcurrentBot(new CmdBot(new BotGuid(Guid.NewGuid()))),
                new ConcurrentBot(new TelegramBot(GetBotToken(), new BotGuid(Guid.NewGuid()))),
			};
			
			return bots;
		}

		private static SecureString GetBotToken()
		{
			var token = SecurityEnvironment.GetPassword("telegram", "bot", "token");
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

            Console.WriteLine("Save token.");
			SecurityEnvironment.SetPassword(token, "telegram", "bot", "token");
			return token;
		}
	}
}