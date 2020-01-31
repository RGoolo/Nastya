using System;
using Model.BotTypes.Enums;

namespace Model.BotTypes.Interfaces
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
