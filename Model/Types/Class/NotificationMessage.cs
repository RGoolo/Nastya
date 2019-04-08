using Model.Types.Enums;
using Model.Types.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Types.Class
{
	public class NotificationMessage : IMessage
	{
		public NotificationMessage(IMessage msg, CommandMessage cMsg)
		{
			ReplyToMessage = msg;
			ReplyToCommandMessage = cMsg;
		}

		public Guid MessageId => ReplyToMessage.MessageId;

		public Guid BotId => ReplyToMessage.BotId;

		public Guid ChatId => ReplyToMessage.ChatId;

		public string Text => null;

		public MessageType TypeMessage => MessageType.SystemMessage;

		public IUser User => new NotificationUser();

		public List<IMessageCommand> MessageCommands => new List<IMessageCommand>();

		public IMessage ReplyToMessage { get; }

		public CommandMessage ReplyToCommandMessage { get; }

		public IResource Resource { get; set; } = null;
	}

	public class NotificationUser : IUser
	{
		public string Display => "Nasty";
		public Guid Id => new Guid("{57ADBED6-167E-48ED-91F2-6EB159A491BA}"); 
		public TypeUser Type => TypeUser.Admin | TypeUser.Developer;
	}
}
