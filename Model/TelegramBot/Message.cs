using Model.Types.Enums;
using System;
using System.Collections.Generic;
using Model.Types.Interfaces;
using System.Linq;
using Model.Types.Class;

namespace Model.TelegramBot
{

	public class TelegramMessage : IMessage
	{
		public Telegram.Bot.Types.Message Message { get; }
		public TypeUser TypeUser { get; }
		public Guid BotId { get; }
		public string Text => Message.Text;

		private readonly CreaterCommands _сreatedCommands = new CreaterCommands("/");
		private IMessage _answerOn;

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
					default:
						return MessageType.Undefined;
				}
			}
		}
		public Guid ChatId => Message.Chat.Id.ToGuid();

		public IUser User { get; }

		public List<IMessageCommand> MessageCommands { get; }

		public Guid MessageId => Message.MessageId.ToGuid();

		public IMessage ReplyToMessage => _answerOn ?? (Message.ReplyToMessage == null ? null : (_answerOn = new TelegramMessage(Message.ReplyToMessage, TypeUser)));

		//ToDo
		public bool ContainsResources => false;

		public TypeResource TypeResource => TypeResource.None;

		//public string ResourcePath { get; set; }
		public IResource Resource { get ; set; }

		public CommandMessage ReplyToCommandMessage => throw new NotImplementedException();

		public TelegramMessage(Telegram.Bot.Types.Message msg, TypeUser typeUser)
		{
			//BotId = botId;
			Message = msg;
			TypeUser = typeUser;
			User = new User(msg.From, typeUser);
			var entityValues = Message.EntityValues;
			if (entityValues != null)
			{
				try
				{
					MessageCommands = _сreatedCommands.CreateCommands(Message.Text, entityValues.Select(x => x.Substring(1)).Select(x => x.Contains("@") ? (x.Split('@')[0]) : x)
							.ToList());
				}
				catch
				{

				}
			}
		}

	}

	
}
