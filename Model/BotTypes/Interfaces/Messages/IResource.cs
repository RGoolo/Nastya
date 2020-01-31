using Model.BotTypes.Enums;
using Model.Files.FileTokens;

namespace Model.BotTypes.Interfaces.Messages
{
	public interface IResource
	{
		IChatFile File { get; }
		TypeResource Type { get; }
	}
}