using System;
using System.Collections.Generic;
using Model.BotTypes.Class;
using Model.BotTypes.Enums;

namespace Model.BotTypes.Interfaces.Messages
{
	public interface IBotMessage
	{
		IMessageId MessageId { get; }
		IChatId ChatId { get; }
		string Text { get; }
		MessageType TypeMessage { get; }
		IUser User { get; }
		List<IMessageCommand> MessageCommands { get;}
		IBotMessage ReplyToMessage { get; }
		IMessageToBot ReplyToCommandMessage { get; }
		IResource Resource { get; set; }
	}
}
