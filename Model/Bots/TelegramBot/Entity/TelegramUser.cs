using System;
using Model.Bots.BotTypes.Class;
using Model.Bots.BotTypes.Enums;
using Model.Bots.BotTypes.Interfaces.Messages;

namespace Model.Bots.TelegramBot.Entity
{
	public class TelegramUser : IUser
	{
		public Telegram.Bot.Types.User TUser { get; }

		public TypeUser Type { get; }

		public string Display => string.IsNullOrEmpty(TUser.FirstName) ? TUser.LastName : TUser.FirstName;

		public Guid Id => IdsMapper.ToGuid(TUser.Id);

		public TelegramUser(Telegram.Bot.Types.User user, TypeUser typeUser)
		{
			TUser = user;
			Type = typeUser;
		}
	}
}
