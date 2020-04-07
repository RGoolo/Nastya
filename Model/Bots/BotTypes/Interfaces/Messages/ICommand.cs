using Model.Bots.BotTypes.Enums;

namespace Model.Bots.BotTypes.Interfaces.Messages
{
	public interface ICommand
	{
		string Value { get; }
		MessageType TypeMessage { get; }
	}
}