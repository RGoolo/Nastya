using System;
using Model.BotTypes.Enums;
using Model.BotTypes.Interfaces;

namespace Model.BotTypes.Attribute
{
	//ToDo: refactor
	[AttributeUsage(AttributeTargets.Class)]
	public class CommandClassAttribute : CommandInfoAttribute,  ITypeUserAttribute
	{
		public bool SaveThread { get; }

		public CommandClassAttribute(string alias, string description, TypeUser typeUser = TypeUser.Admin, bool saveThread = false)
			: base( typeUser, alias, description)
		{
			SaveThread = saveThread;
		}
	}

	public class CustomHelpAttribute : System.Attribute
	{
		public string CustomHelp{ get; }

		public CustomHelpAttribute(string customHelp)
		{
			CustomHelp = customHelp;
		}
	}


	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
	public class CommandAttribute : CommandInfoAttribute, ITypeUserAttribute
	{
		public TypeResource Resource { get; }

		public CommandAttribute(string alias, string description, TypeUser typeUser = TypeUser.Admin, TypeResource resource = TypeResource.None)
			: base(typeUser, alias, description)
		{
			Resource = resource;
		}
	}
	
	[AttributeUsage(AttributeTargets.Method)]
	public class CommandOnMsgAttribute : MessageAttribute, ICheckAttribute, ITypeUserAttribute
	{
		public CommandOnMsgAttribute(string boolPropertyName, MessageType typeMessages, TypeUser typeUser = TypeUser.Admin)
			: base(boolPropertyName, typeMessages, typeUser)
		{
		}
	}


	[AttributeUsage(AttributeTargets.Method)]
	public class CommandSystemAttribute : MessageAttribute, ICheckAttribute, ITypeUserAttribute
	{
		public SystemType SType { get; }
		public CommandSystemAttribute(string boolPropertyName, MessageType typeMessages, SystemType sType, TypeUser typeUser = TypeUser.Admin)
			: base(boolPropertyName, typeMessages, typeUser)
		{
			SType = sType;
		}
	}

	[AttributeUsage(AttributeTargets.Method)]
	public abstract class MessageAttribute : CheckPropertyAttribute, ICheckAttribute, ITypeUserAttribute
	{
		public MessageType TypeMessages { get; }
		public TypeUser TypeUser { get; }

		protected MessageAttribute(string boolPropertyName, MessageType typeMessages, TypeUser typeUser = TypeUser.Admin)
		: base(boolPropertyName)
		{
			TypeMessages = typeMessages;
			TypeUser = typeUser;
		}
	}

	//ToDo: Alias and Description to DisplayAttribute
	public abstract class CommandInfoAttribute : System.Attribute, ITypeUserAttribute
	{
		public TypeUser TypeUser { get; }
		public string Alias { get; }
		public string Description { get; }

		protected CommandInfoAttribute(TypeUser typeUser, string alias, string description)
		{
			Alias = alias;
			Description = description;
			TypeUser = typeUser;
		}
	}

	public abstract class CommandPayAttribute : System.Attribute, IPay
	{
		public Guid Guid { get; }
		public bool Paid { get; }

		protected CommandPayAttribute(Guid guid, bool paid)
		{
			Guid = guid;
			Paid = paid;
		}
	}

	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true)]
	public class CheckPropertyAttribute : System.Attribute, ICheckAttribute
	{
		public string BoolPropertyName { get; protected set; }

		public CheckPropertyAttribute(string boolPropertyName)
		{
			BoolPropertyName = boolPropertyName;
		}
	}

	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
	public class ShortHelpAttribute : System.Attribute
	{
		public ShortHelpAttribute()
		{

		}
	}
}

