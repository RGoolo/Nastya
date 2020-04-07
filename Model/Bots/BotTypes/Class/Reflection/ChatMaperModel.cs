using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Model.Bots.BotTypes.Attribute;
using Model.Bots.BotTypes.Enums;
using Model.Bots.BotTypes.Interfaces;
using Model.Bots.BotTypes.Interfaces.Ids;
using Model.Bots.BotTypes.Interfaces.Messages;
using Model.Files.FileTokens;
using Model.Logic.Model;
using Model.Logic.Settings;
using ILogger = Model.Logger.ILogger;
using IUser = Model.Bots.BotTypes.Interfaces.Messages.IUser;

namespace Model.Bots.BotTypes.Class.Reflection
{
	public class MapperPropInfo : MapperMemberInfo, IPay
	{
		public PropertyInfo _propInfo { get; }
		public bool IsPassword { get; }

		public MapperPropInfo(PropertyInfo propInfo, object instance) : base(propInfo, instance)
		{
			_propInfo = propInfo;
			IsPassword = propInfo.GetCustomAttribute<PasswordAttribute>() != null;
		}

		public void SetDefault() => SetValue(StandardStructureMapper.GetDefault(_propInfo.PropertyType));
		public void SetValue(string value) => SetValue(GetValuesWithEnum(_propInfo.PropertyType, value));

		public bool IsGet => _propInfo.CanRead;
		public object Get => _propInfo.GetValue(Instance);

		private void SetValue(object value)
		{
			try
			{
				_propInfo.SetValue(Instance, value);
			}
			catch(Exception ex)
			{
				{ Logger.Logger.CreateLogger(nameof(MapperPropInfo)).Warning(ex); }
				throw new ModelException($"Не удалось поменять значение на \"{value}\".");
			}
		}
	}

	public class MapperMethodOnAllMsg : MapperMethodInfo, IPay
	{
		private CommandOnMsgAttribute CommandOnMsg { get; }

		public MapperMethodOnAllMsg(MethodInfo methodInfo,  object instance) : base(methodInfo, instance)
		{
			CommandOnMsg = methodInfo.GetCustomAttribute<CommandOnMsgAttribute>(true);
		}

		public object Invoke(IBotMessage msg) => Invoke(msg, null);
		public override bool CheckUsage(IBotMessage msg) => (CommandOnMsg.TypeMessages & msg.TypeMessage) == msg.TypeMessage && base.CheckUsage(msg);
	}

	public class MapperMethodInfo : BaseMapperMethodInfo, IPay
	{
		public CommandAttribute CommandAttribute;

		public MapperMethodInfo(MethodInfo methodInfo, object instance) : base(methodInfo, instance)
		{
			CommandAttribute = methodInfo.GetCustomAttribute<CommandAttribute>(true);
		}
	}

	public abstract class BaseMapperMethodInfo : MapperMemberInfo
	{
		protected MethodInfo MethodInfo { get; }
		public ILogger Logger = Model.Logger.Logger.CreateLogger(nameof(BaseMapperMethodInfo));

		protected BaseMapperMethodInfo(MethodInfo methodInfo, object instance) : base(methodInfo, instance)
		{
			MethodInfo = methodInfo;
		}

		public object Invoke(IBotMessage msg, IMessageCommand msgCommand)
		{
			try
			{
				return InvokeMethodWithCastParam(msg, msgCommand);
			}
			catch(ModelException model)
			{
				throw;
			}
			catch (Exception ex)
			{
				if (ex.InnerException is ModelException)
					throw ex.InnerException;

				Logger.Error(ex);
				throw new MessageException(msg, $"Не удалось выполнить метод." );
			}
		}

		private static Dictionary<Type, Func<IBotMessage, IMessageCommand, object>> parametersDic => new Dictionary<Type, Func<IBotMessage, IMessageCommand, object>>()
		{
			[typeof(string[])] = (mess, command) => command.Values.ToArray(),
			[typeof(IChatFile)] = (mess, command) => mess.Resource.File,
			[typeof(IChatFile)] = (mess, command) => mess.Resource.File,
				
			[typeof(IEnumerable<string>)] = (mess, command) => command.Values,
			[typeof(IMessageCommand)] = (mess, command) => command,
			[typeof(IBotMessage)] = (mess, command) => mess,
			[typeof(IUser)] = (mess, command) => mess.User,

			[typeof(IChatId)] = (mess, command) => mess.Chat.Id,
			[typeof(ISettings)] = (mess, command) => SettingsHelper.GetSetting(mess.Chat.Id),
			[typeof(IChatFileFactory)] = (mess, command) => SettingsHelper.GetSetting(mess.Chat.Id).FileChatFactory,
			[typeof(IMessageToBot)] = (mess, command) => mess.ReplyToCommandMessage,
			[typeof(IMessageId)] = (mess, command) => mess.MessageId,
			//[typeof(ISendMessage)] = (mess, command) => mess.User,
		};

		private object InvokeMethodWithCastParam(IBotMessage msg, IMessageCommand msgCommand)
		{
			var parameters = new List<object>();
			var i = 0;

			foreach (var param in MethodInfo.GetParameters())
			{
				if (parametersDic.TryGetValue(param.ParameterType, out var f))
					parameters.Add(f(msg, msgCommand));
				else
				{
					if (i < msgCommand.Values.Count())
					{
						parameters.Add(GetValuesWithEnum(param.ParameterType, msgCommand.Values[i]));
						i++;
					}
					else if (param.IsOptional) 
						parameters.Add(param.DefaultValue);
					else
					{
						var desc = param.GetCustomAttribute<DescriptionAttribute>();
						throw new ArgumentNeedException(desc?.Description ?? param.Name);
					}
						
					// parameters.Add(StandardStructureMapper.GetDefault(param.ParameterType));
					
				}
			}

			return MethodInfo.Invoke(Instance, parameters.ToArray());
		}
	}

	public abstract class MapperMemberInfo : IPay
	{
		public string Name { get; }
		public CommandClassAttribute InstanceAttribute { get; }
		public Guid Guid { get; }
		public bool Paid { get; }
		protected object Instance { get; }
		protected MemberInfo MemberInfo { get; }
		protected TypeUser AccessUser { get; }
		protected List<PropertyInfo> CheckAttributes  { get; }

		protected MapperMemberInfo(MemberInfo memberInfo, object instance)
		{
			Instance = instance;
			MemberInfo = memberInfo;
			InstanceAttribute = instance.GetType().GetCustomAttribute<CommandClassAttribute>(true);
			AccessUser = memberInfo.GetCustomAttributes(true).OfType<ITypeUserAttribute>().First().TypeUser;

			var paid = memberInfo.GetCustomAttributes(true).OfType<IPay>().FirstOrDefault();
			Guid = paid?.Guid ?? default(Guid);
			Paid = paid != null;

			CheckAttributes = new List<PropertyInfo>();
			FillCheckAttributes();
		}

		private void FillCheckAttributes()
		{
			
			var checkPropNames = MemberInfo.GetCustomAttributes(true)?.OfType<ICheckAttribute>()?.Select(x => x.BoolPropertyName)?.ToList();
			foreach (var checkPropName in checkPropNames)
				CheckAttributes.AddRange(Instance.GetType().GetProperties().Where(x => x.Name == checkPropName && x.PropertyType == typeof(bool)));
		}

		public virtual bool CheckUsage(IBotMessage msg)
		{
			if (!NeedUse(msg))
				return false;

			if (CheckAccess(msg.User.Type))
				return true;

			if (msg.User.Type.IsBot())
				return false;

			if (AccessUser.IsAdmin() && !msg.User.Type.IsAdmin())
				throw new MessageException(msg, $"Доступно только для администраторов группы.");

			if (AccessUser.IsDeveloper() && !msg.User.Type.IsDeveloper())
				throw new MessageException(msg, $"Доступно только для разработчиков.");

			return false;
		}

		protected bool CheckAccess(TypeUser user) => AccessChecker.CheckAccess(InstanceAttribute.TypeUser, AccessUser, user);
		protected virtual bool NeedUse(IBotMessage msg) =>	(CheckAttributes.All(x => (bool)x.GetValue(Instance)));
		protected object GetValuesWithEnum(Type type, string value) => StandardStructureMapper.GetType(type, value);
	}
}
