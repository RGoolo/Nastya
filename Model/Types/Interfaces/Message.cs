using Model.Types.Class;
using Model.Types.Enums;
using System;
using System.Collections.Generic;

namespace Model.Types.Interfaces
{
	public interface IMessage
	{
		Guid MessageId { get; }
		Guid BotId { get; }
		Guid ChatId { get; }
		string Text { get; }
		MessageType TypeMessage { get; }
		IUser User { get; }
		List<IMessageCommand> MessageCommands { get;}
		IMessage ReplyToMessage { get; }
		CommandMessage ReplyToCommandMessage { get; }
		IResource Resource { get; set; }
	}

	public interface IResource
	{
		IFileToken File { get; }
		TypeResource Type { get; }
	}

	public interface IMessageCommand
	{
		bool SetDefaultValue { get; }

		string Name { get; }
		string FirstValue { get; }
		List<string> Values { get; }
	}

	public interface IUser
	{
		TypeUser Type { get;}
		string Display { get; }
		Guid Id { get; }
	}

	public interface ICommand
	{
		string Value { get; }
		MessageType TypeMessage { get; }
	}
}
