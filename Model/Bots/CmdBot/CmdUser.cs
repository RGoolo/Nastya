using System;
using Model.Bots.BotTypes.Enums;
using Model.Bots.BotTypes.Interfaces.Messages;

namespace Model.Bots.CmdBot
{
	public class CmdUser : IUser
	{
		public string Display => "Пользователь ПК";
		public Guid Id => new Guid("{75C20E89-B048-4EF6-8731-922E6DE587BA}");
		public TypeUser Type => TypeUser.Admin | TypeUser.Developer;
	}
}