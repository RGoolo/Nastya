using Model.BotTypes.Enums;
using Model.BotTypes.Interfaces.Messages;
using Model.Files.FileTokens;

namespace Model.BotTypes.Interfaces
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