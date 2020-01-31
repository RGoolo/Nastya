using Model.BotTypes.Enums;

namespace Model.BotTypes.Interfaces.Messages
{
	public interface ICommand
	{
		string Value { get; }
		MessageType TypeMessage { get; }
	}
}