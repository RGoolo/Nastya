using System;
using Model.BotTypes.Enums;

namespace Model.BotTypes.Interfaces.Messages
{
	public interface IUser
	{
		TypeUser Type { get;}
		string Display { get; }
		Guid Id { get; }
	}
}