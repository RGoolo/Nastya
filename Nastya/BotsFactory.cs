using System;
using System.Collections.Generic;
using System.Security;
using BotModel.Bots.BotTypes.Class;
using BotModel.Bots.BotTypes.Class.Ids;
using BotModel.Bots.BotTypes.Interfaces;
using BotModel.Bots.BotTypes.Interfaces.Messages;
using BotModel.Bots.CmdBot;
using BotModel.Bots.TelegramBot.Entity;
using Model.Services;

namespace NightGameBot
{
	public static class BotsFactory
	{
		public static List<IBot> Bots()
		{
			var bots = new List<IBot>
			{
				new ConcurrentBot(new CmdBot(new BotGuid(Guid.NewGuid()))),
                new ConcurrentBot(new TelegramBot(GetBotToken(), new BotGuid(Guid.NewGuid()), new TexterService())),
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

            var ss = new SecureString();
            foreach (var c in "484932128:AAH8OYJjTQ4dGmXzPvWkDQChLsq7zVE_h38")
                ss.AppendChar(c);
            return ss;

			/*Console.WriteLine("Write a telegram bot token:");
			token = new SecureString();

			var key = Console.ReadKey(true);
			while (key.Key != ConsoleKey.Enter)
			{
				token.AppendChar(key.KeyChar);
				key = Console.ReadKey(true);
			}

            Console.WriteLine("Saved token.");
			SecurityEnvironment.SetPassword(token, "telegram", "bot", "token");
			return token;*/
		}
	}
}