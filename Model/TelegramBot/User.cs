using System;
using Model.BotTypes.Class;
using Model.BotTypes.Enums;
using Model.BotTypes.Interfaces;
using Model.BotTypes.Interfaces.Messages;

namespace Model.TelegramBot
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
