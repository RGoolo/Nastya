using Model.Bots.BotTypes.Class.Ids;
using Model.Bots.BotTypes.Interfaces;
using Model.Bots.BotTypes.Interfaces.Ids;

namespace Model.Bots.CmdBot
{
	public class CmdChat : IChat
	{
		public string ChatName => "cmd chat";
		public ChatType Type => ChatType.Private;
		public IChatId Id => new ChatGuid("BBAF99E5-EF91-4FD4-BA25-4A111A071111");
	}
}