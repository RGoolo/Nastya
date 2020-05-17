using System;
using Model.Bots.BotTypes.Class.Ids;
using Model.Bots.BotTypes.Interfaces;
using Model.Bots.BotTypes.Interfaces.Ids;

namespace Model.Bots.UnitTestBot
{
	public class UnitTestChat : IChat
	{
        public string ChatName { get; } = "unit test chat";
        public ChatType Type { get; set; } = ChatType.Private;
        public IChatId Id { get; set; } = new ChatGuid(new Guid());
    }
}