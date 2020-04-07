using System.Collections.Generic;
using System.Linq;
using Model.Bots.BotTypes.Class;
using Model.Bots.BotTypes.Class.Ids;
using Model.Bots.BotTypes.Enums;
using Model.Bots.BotTypes.Interfaces;
using Model.Bots.BotTypes.Interfaces.Ids;
using Model.Bots.BotTypes.Interfaces.Messages;
using Model.Bots.TelegramBot.Services;
using Model.Logic.Settings;
using Telegram.Bot.Types;

namespace Model.Bots.TelegramBot.Entity
{

	public class TelegramMessage : IBotMessage
	{
		public Message Message { get; }
		public IChat Chat { get; }
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

		public IUser User { get; }

		public List<IMessageCommand> MessageCommands { get; private set; }

		public IMessageId MessageId => new MessageInt(Message.MessageId, Chat.Id);

		public IBotMessage ReplyToMessage => _answerOn ?? (Message.ReplyToMessage == null ? null : (_answerOn = new TelegramMessage(Message.ReplyToMessage, TypeUser.User))); //ToDo change type user

		public IResource Resource { get ; set; }

		public IMessageToBot ReplyToCommandMessage { get; } 

		public TelegramMessage(Message msg, TypeUser typeUser, IMessageToBot message = null)
		{
			Chat = new TelegramChat(msg.Chat);

			Message = msg;
			User = new TelegramUser(msg.From, typeUser);
			if ((typeUser & TypeUser.Bot) == 0)
				TryCreateCommand(msg);

			ReplyToCommandMessage = message;
			
			if (message?.FileToken != null && (message.TypeMessage & MessageType.WithResource) != MessageType.Undefined)
			{
				var setting = SettingsHelper.GetSetting(Chat.Id);
				var file = setting.FileChatFactory.GetChatFile (message.FileToken);
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
