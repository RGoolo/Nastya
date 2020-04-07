using System;
using Model.Bots.BotTypes.Enums;

namespace Model.Bots.BotTypes.Interfaces
{
	public interface IPay
	{
		Guid Guid { get; }
		bool Paid { get; }
	}

	public interface ICheckAttribute 
	{
		string BoolPropertyName { get; }
	}

	public interface ITypeUserAttribute
	{
		TypeUser TypeUser { get; }
	}
}
