using System.Collections.Generic;

namespace Model.Bots.BotTypes.Interfaces.Messages
{
	public interface IMessageCommand
	{
		bool SetDefaultValue { get; }
		string Name { get; }
		string FirstValue { get; }
		List<string> Values { get; }
	}
}