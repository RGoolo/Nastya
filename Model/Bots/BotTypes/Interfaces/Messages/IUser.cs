using System;
using Model.Bots.BotTypes.Enums;

namespace Model.Bots.BotTypes.Interfaces.Messages
{
	public interface IUser
	{
		TypeUser Type { get;}
		string Display { get; }
		Guid Id { get; }
	}
}