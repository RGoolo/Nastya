using Model.Bots.BotTypes.Enums;
using Model.Bots.BotTypes.Interfaces.Messages;
using Model.Files.FileTokens;

namespace Model.Bots.BotTypes.Interfaces
{
	public class Resource : IResource
	{
		public Resource(IChatFile file, TypeResource type)
		{
			File = file;
			Type = type;
		}

		public IChatFile File { get; }
		public TypeResource Type { get; }
	}
}