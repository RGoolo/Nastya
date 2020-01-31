using System;
using System.Collections.Generic;
using Model.BotTypes.Enums;
using Model.BotTypes.Interfaces;
using Model.BotTypes.Interfaces.Messages;

namespace Model.BotTypes.Class
{
	public class NotificationMessage : IBotMessage
	{
		public NotificationMessage(IBotMessage msg, IMessageToBot cMsg)
		{
			ReplyToMessage = msg;
			ReplyToCommandMessage = cMsg;
		}

		public IMessageId MessageId =>ReplyToMessage.MessageId;

		// public IBotId BotId => ReplyToMessage.BotId;

		public IChatId ChatId => ReplyToMessage.ChatId;

		public string Text => null;

		public MessageType TypeMessage => MessageType.SystemMessage;

		public IUser User => new NotificationUser();

		public List<IMessageCommand> MessageCommands => new List<IMessageCommand>();

		public IBotMessage ReplyToMessage { get; }

		public IMessageToBot ReplyToCommandMessage { get; }

		public IResource Resource { get; set; } = null;
	}

	public class NotificationUser : IUser
	{
		public string Display => "Nasty";
		public Guid Id => new Guid("{57ADBED6-167E-48ED-91F2-6EB159A491BA}"); 
		public TypeUser Type => TypeUser.Admin | TypeUser.Developer;
	}
}
