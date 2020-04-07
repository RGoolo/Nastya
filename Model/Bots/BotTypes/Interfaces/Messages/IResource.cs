using Model.Bots.BotTypes.Enums;
using Model.Files.FileTokens;

namespace Model.Bots.BotTypes.Interfaces.Messages
{
	public interface IResource
	{
		IChatFile File { get; }
		TypeResource Type { get; }
	}
}