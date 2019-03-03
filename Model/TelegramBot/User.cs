using Model.Types.Enums;
using Model.Types.Interfaces;
using System;
using Model.Types.Class;

namespace Model.TelegramBot
{
	public class User : IUser
	{
		public Telegram.Bot.Types.User TUser { get; }

		public TypeUser Type { get; }

		public string Display => string.IsNullOrEmpty(TUser.FirstName) ? TUser.LastName : TUser.FirstName;

		public Guid Id => TUser.Id.ToGuid();

		public User(Telegram.Bot.Types.User user, TypeUser typeUser)
		{
			TUser = user;
			Type = typeUser;
		}
	}
}
