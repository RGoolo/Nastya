﻿using System;
using System.Collections.Generic;
using System.Linq;
using Model.BotTypes.Class;
using Model.BotTypes.Class.Ids;
using Model.BotTypes.Enums;
using Model.BotTypes.Interfaces;
using Model.BotTypes.Interfaces.Messages;
using Model.Logic.Settings;
using Telegram.Bot.Types;

namespace Model.TelegramBot
{

	public class TelegramMessage : IBotMessage
	{
		public Telegram.Bot.Types.Message Message { get; }
		public TypeUser TypeUser { get; }
		public string Text => Message.Text;

		private readonly CreatorCommands _сreatorCommands = new CreatorCommands("/");
		private IBotMessage _answerOn;

		public MessageType TypeMessage
		{
			get
			{
				switch (Message.Type)
				{
					case Telegram.Bot.Types.Enums.MessageType.Photo:
						return MessageType.Photo;
					case Telegram.Bot.Types.Enums.MessageType.Voice:
						return MessageType.Voice;
					case Telegram.Bot.Types.Enums.MessageType.Video:
						return MessageType.Video;
					case Telegram.Bot.Types.Enums.MessageType.Text:
						return MessageType.Text;
					case Telegram.Bot.Types.Enums.MessageType.Document:
						return MessageType.Document;
					case Telegram.Bot.Types.Enums.MessageType.Venue:
						return MessageType.Coordinates; //ToDo or add new?
					default:
						return MessageType.Undefined;
				}
			}
		}

		public IChatId ChatId { get; }

		public IUser User { get; }

		public List<IMessageCommand> MessageCommands { get; private set; }

		public IMessageId MessageId => new MessageInt(Message.MessageId);

		public IBotMessage ReplyToMessage => _answerOn ?? (Message.ReplyToMessage == null ? null : (_answerOn = new TelegramMessage(Message.ReplyToMessage, TypeUser)));

		public IResource Resource { get ; set; }

		public IMessageToBot ReplyToCommandMessage { get; } // throw new NotImplementedException(); ToDo

		public TelegramMessage(Message msg, TypeUser typeUser, IMessageToBot message = null)
		{
			ChatId = new ChatLong(msg.Chat.Id);

			Message = msg;
			TypeUser = typeUser;
			User = new TelegramUser(msg.From, typeUser);
			if ((typeUser & TypeUser.Bot) == 0)
				TryCreateCommand(msg);

			ReplyToCommandMessage = message;
			
			if (message?.FileToken != null && (message.TypeMessage & MessageType.WithResource) != MessageType.Undefined)
			{
				var setting = SettingsHelper.GetSetting(ChatId);
				var file = setting.FileChatFactory.GetChatFile(message.FileToken);
				Resource = new Resource(file, message.TypeMessage.Convert());
			}
		}

		private void TryCreateCommand(Telegram.Bot.Types.Message msg)
		{
			var entityValues = Message.EntityValues;
			if (entityValues != null)
			{
				try
				{
					MessageCommands = _сreatorCommands.CreateCommands(Message.Text, entityValues.Select(x => x.Substring(1)).Select(x => x.Contains("@") ? (x.Split('@')[0]) : x)
						.ToList());
				}
				catch
				{

				}
			}
		}

	}
}
