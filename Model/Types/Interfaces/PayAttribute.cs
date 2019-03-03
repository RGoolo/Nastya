using Model.Types.Enums;
using System;

namespace Model.Types.Interfaces
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
