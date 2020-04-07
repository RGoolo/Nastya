using Model.Bots.BotTypes.Class.Ids;
using Model.Bots.BotTypes.Interfaces;
using Model.Bots.BotTypes.Interfaces.Ids;
using Telegram.Bot.Types;

namespace Model.Bots.TelegramBot.Entity
{
	public class TelegramChat : IChat
	{
		public TelegramChat(Chat chat)
		{
			ChatName = chat.Title;
			Id =  new ChatLong(chat.Id);
			Type = chat.Type switch
			{
				Telegram.Bot.Types.Enums.ChatType.Private => ChatType.Private,
				_ => ChatType.Group,
			};
		}

		public string ChatName { get; }
		public ChatType Type { get; }
		public IChatId Id { get; }
	}
}