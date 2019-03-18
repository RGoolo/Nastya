using Model.Types.Enums;
using Model.Types.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Model.Logic.Model;
using Model.Types.Attribute;

namespace Model.Types.Class
{
	public class MaperPropInfo : MaperMemberInfo, IPay
	{
		public PropertyInfo _propInfo { get; }

		public MaperPropInfo(PropertyInfo propInfo, object instance) : base(propInfo, instance)
		{
			_propInfo = propInfo;
		}

		public void SetDefault() => SetValue(GetDefault(_propInfo.PropertyType));
		public void SetValue(string value) => SetValue(GetValuesWithEnum(_propInfo.PropertyType, value));

		private void SetValue(object value)
		{
			try
			{
				_propInfo.SetValue(Instance, value);
			}
			catch
			{
				throw new ModelException($"Не удалось поменять значение на \"{value}\".");
			}
		}
	}

	public class MaperMethodOnAllMsg : MaperMethodInfo, IPay
	{
		private CommandOnMsgAttribute _commandOnMsg { get; }

		public MaperMethodOnAllMsg(MethodInfo methodInfo,  object instance) : base(methodInfo, instance)
		{
			_commandOnMsg = methodInfo.GetCustomAttribute<CommandOnMsgAttribute>(true);
		}

		public object Invoke(IMessage msg) => Invoke(msg, null);

		public override bool СheckUsage(IMessage msg) => (_commandOnMsg.TypeMessages & msg.TypeMessage) == msg.TypeMessage && base.СheckUsage(msg);
	}

	public class MaperMethodInfo : BaseMaperMethodInfo, IPay
	{
		public CommandAttribute CommandAttribute;

		public MaperMethodInfo(MethodInfo methodInfo, object instance) : base(methodInfo, instance)
		{
			CommandAttribute = methodInfo.GetCustomAttribute<CommandAttribute>(true);
		}
	}

	public abstract class BaseMaperMethodInfo : MaperMemberInfo
	{
		protected MethodInfo _methodInfo { get; }
		protected BaseMaperMethodInfo(MethodInfo methodInfo, object instance) : base(methodInfo, instance)
		{
			_methodInfo = methodInfo;
		}

		public object Invoke(IMessage msg, IMessageCommand msgCommand)
		{
			try
			{
				return InvokeMethodWithCastParam(msg, msgCommand);
			}
			catch
			{
				throw new MessageException(msg, $"Не удалось вызвать метод.");
			}
		}

		private object InvokeMethodWithCastParam(IMessage msg, IMessageCommand msgCommand)
		{
			var parametrs = new List<object>();
			int i = 0;

			foreach (var param in _methodInfo.GetParameters())
			{
				if (param.ParameterType == typeof(string[]))
					parametrs.Add(msgCommand.Values.ToArray());
				else if (param.ParameterType == typeof(IFileToken))
					parametrs.Add(msg.Resource.File);
				else if (param.ParameterType == typeof(IEnumerable<string>))
					parametrs.Add(msgCommand.Values);
				else if (param.ParameterType == typeof(IMessageCommand))
					parametrs.Add(msgCommand);
				else if (param.ParameterType == typeof(IMessage))
					parametrs.Add(msg);
				else
				{
					if (i < msgCommand.Values.Count())
					{
						if (param.IsOptional)
							parametrs.Add(Type.Missing);	
						else
							//ToDo: error?
							parametrs.Add(GetValuesWithEnum(param.ParameterType, msgCommand.Values[i]));
					}
					else
						parametrs.Add(GetDefault(param.ParameterType));
					i++;
				}
			}
			return _methodInfo.Invoke(Instance, parametrs.ToArray());
		}
	}

	public abstract class MaperMemberInfo : IPay
	{
		public string Name { get; }
		public CommandClassAttribute InstanceAttribute;
		public Guid Guid { get; }
		public bool Paid { get; }

		protected object Instance { get; }
		protected MemberInfo _memberInfo { get; }
		protected TypeUser _accessUser { get; }
		protected List<PropertyInfo> _checkAttributes;

		protected MaperMemberInfo(MemberInfo memberInfo, object instance)
		{
			Instance = instance;
			_memberInfo = memberInfo;
			InstanceAttribute = instance.GetType().GetCustomAttribute<CommandClassAttribute>(true);
			_accessUser = memberInfo.GetCustomAttributes(true).OfType<ITypeUserAttribute>().First().TypeUser;

			var paid = memberInfo.GetCustomAttributes(true).OfType<IPay>().FirstOrDefault();
			Guid = paid?.Guid ?? default(Guid);
			Paid = paid != null;

			FillCheckAttributes();
		}

		private void FillCheckAttributes()
		{
			_checkAttributes = new List<PropertyInfo>();

			var checkPropNames = _memberInfo.GetCustomAttributes(true)?.OfType<ICheckAttribute>()?.Select(x => x.BoolPropertyName)?.ToList();
			foreach (var checkPropName in checkPropNames)
				_checkAttributes.AddRange(Instance.GetType().GetProperties().Where(x => x.Name == checkPropName && x.PropertyType == typeof(bool)));
		}

		public virtual bool СheckUsage(IMessage msg)
		{
			if (!NeedUse(msg))
				return false;

			if (!CheckAccess(msg.User.Type))
			{
				//ToDo:  
				if (AccessChecker.ContainsType(msg.User.Type, TypeUser.Admin))
					throw new MessageException(msg, $"Доступно только для разработчика.");
				throw new MessageException(msg, $"Доступно только для администраторов группы.");
			}
			return true;
		}

		protected bool CheckAccess(TypeUser user) => AccessChecker.CheckAccess(InstanceAttribute.TypeUser, _accessUser, user);
		
		protected virtual bool NeedUse(IMessage msg) =>	(_checkAttributes.All(x => (bool)x.GetValue(Instance)));
	
		protected object GetValuesWithEnum (Type type, string value) => type.IsEnum ? Enum.Parse(type, value) : GetValues[type](value);

		private static readonly Dictionary<Type, Func<string, object>> GetValues = new Dictionary<Type, Func<string, object>>()
		{
			{ typeof(Guid?), (value) => GetGuid(value)},
			{ typeof(Guid), (value) => new Guid(value)},
			{ typeof(int?), (value) => GetInt(value)},
			{ typeof(int), (value) => int.Parse(value)},
			{ typeof(long?), (value) => GetLong(value)},
			{ typeof(long), (value) => long.Parse(value)},
			{ typeof(DateTime?), (value) => GetDateTime(value)},
			{ typeof(DateTime), (value) => DateTime.Parse(value)},
			{ typeof(bool?), (value) => GetBool(value)},
			{ typeof(bool), (value) => bool.Parse(value)},
			{ typeof(string), (value) => value},
		};

		protected static object GetDefault(Type type) => type.IsValueType ? Activator.CreateInstance(type) : null;
		protected static Guid? GetGuid(string value) => string.IsNullOrEmpty(value) ? (Guid?) null : new Guid(value);
		protected static int? GetInt(string value) => string.IsNullOrEmpty(value)? (int?) null : int.Parse(value);
		protected static long? GetLong(string value) => string.IsNullOrEmpty(value) ? (long?) null : long.Parse(value);
		protected static DateTime? GetDateTime(string value) => string.IsNullOrEmpty(value) ? (DateTime?) null : DateTime.Parse(value);
		protected static bool? GetBool(string value) => string.IsNullOrEmpty(value) ? (bool?) null : bool.Parse(value);
	}
}
