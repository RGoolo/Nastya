using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Model.Bots.BotTypes.Enums;
using Model.Bots.TelegramBot.Entity;
using Model.Logger;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Model.Bots.TelegramBot.Services
{
	public class ChatMembersService
	{
		private readonly TelegramBotClient _bot;
		private readonly CancellationToken _cancellationToken;
		private readonly Dictionary<long, ChatAdministrations> _chatAdministations = new Dictionary<long, ChatAdministrations>();
		private const int UpdatetimesTimeSeconds = 300;
		private const int MyUserId = 62779148;
		private readonly ILogger _log;

		public ChatMembersService(TelegramBotClient bot, CancellationToken cancellationToken)
		{
			_bot = bot;
			_cancellationToken = cancellationToken;
		}

		public TypeUser GetTypeUser(Message msg, bool isBot)
		{
			if (isBot)
				return TypeUser.Bot;

			if (msg.Chat.Type == ChatType.Private)
				return GetTypeUser(true, msg);

			if (!_chatAdministations.ContainsKey(msg.Chat.Id) || (DateTime.Now - _chatAdministations[msg.Chat.Id].LastUpdate).TotalSeconds > UpdatetimesTimeSeconds)
			{
				var admins = _bot.GetChatAdministratorsAsync(msg.Chat.Id, _cancellationToken).Result;

				var chatAdmins = new ChatAdministrations();
				chatAdmins.UserIds.AddRange(admins.Select(x => x.User.Id));
				chatAdmins.LastUpdate = DateTime.Now;

				if (_chatAdministations.ContainsKey(msg.Chat.Id))
					_chatAdministations[msg.Chat.Id] = chatAdmins;
				else
					_chatAdministations.TryAdd(msg.Chat.Id, chatAdmins);
			}

			return GetTypeUser(_chatAdministations[msg.Chat.Id].UserIds.Contains(msg.From.Id), msg);
		}

		private TypeUser GetTypeUser(bool isAdmin, Message msg)
		{
			var userType = TypeUser.User;

			if (msg.From.Id == MyUserId)
				userType |= TypeUser.Developer;

			if (isAdmin)
				userType |= TypeUser.Admin;

			return userType;
		}
	}
}