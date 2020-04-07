using System.Collections.Generic;
using Model.Bots.BotTypes.Class;
using Model.Bots.BotTypes.Enums;
using Model.Bots.BotTypes.Interfaces.Ids;

namespace Model.Bots.BotTypes.Interfaces.Messages
{
	public interface IBotMessage
	{
		IMessageId MessageId { get; }
		IChat Chat { get; }

		string Text { get; }
		MessageType TypeMessage { get; }
		IUser User { get; }
		List<IMessageCommand> MessageCommands { get;}
		IBotMessage ReplyToMessage { get; }


		IMessageToBot ReplyToCommandMessage { get; }
		IResource Resource { get; set; }
	}
}
