using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Model.BotTypes;
using Model.BotTypes.Attribute;
using Model.BotTypes.Class;
using Model.BotTypes.Class.Reflection;
using Model.BotTypes.Enums;
using Model.BotTypes.Interfaces.Messages;
using Model.Logic.Model;
using Model.Logic.Settings;
using Nastya.Commands;

namespace Nastya.Mappers
{
	public class ChatMapper 
	{
		private List<object> _instances = new List<object>();
		public PayManager PayManager;
		private readonly TypeBot _typeBot;
		

		private readonly List<IOnMessage> _onMessages = new List<IOnMessage>();
		public IChatId ChatId { get; }

		public ChatMapper(TypeBot typeBot, IChatId chatId, ISendMessages sendMsg)
		{
			ChatId = chatId;
			_typeBot = typeBot;

			var settingHelper = SettingsHelper.GetSetting(chatId);
			
			var all = new MethodsAllMapper(sendMsg, settingHelper);
			var concrete = new MethodsConcreteMapper(sendMsg, settingHelper);
			var props = new PropsMapper(sendMsg, settingHelper);
			
			_onMessages.Add(all);
			_onMessages.Add(concrete);
			_onMessages.Add(props);
			
			PayManager = new PayManager(chatId);

			var allClasses = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.GetCustomAttribute<CommandClassAttribute>(true) != null).ToList();

			var settings = SettingsHelper.GetSetting(chatId);

			foreach (var instance in allClasses.Select(c => CreateInstance(c, sendMsg, chatId, settings)))
			{
				_instances.Add(instance);
				if (instance is BaseCommand baseCommand)
					baseCommand.SendMsg = sendMsg;
				
				all.AddInstance(instance);
				props.AddInstance(instance);
				concrete.AddInstance(instance);
			}

			props.FillProperty();
		}

		private object CreateInstance(Type type, ISendMessages message, IChatId chatId, ISettings settings)
		{
			//ToDo FirstOrDefault?
			var ctors = type.GetConstructors();
			var ctor = ctors.First();
			var param = new List<object>();
			
			foreach (var parameterInfo in ctor.GetParameters())
			{
				if (parameterInfo.ParameterType == typeof(ISendMessages))
					param.Add(message);
				else if (parameterInfo.ParameterType == typeof(IChatId))
					param.Add(chatId);
				else if (parameterInfo.ParameterType == typeof(ISettings))
					param.Add(settings);
			}

			return Activator.CreateInstance(type, param.ToArray());
		}

		public List<TransactionCommandMessage> OnMessage(IBotMessage message)
		{
			return _onMessages.SelectMany(x => x.OnMessage(message)).ToList();
		}

		public void Dispose()
		{
			foreach (var instance in _instances)
			{
				if (instance is BaseCommand baseCommand)
				{
					//baseCommand.SendMsg -= BaseCommand_SendMsg;
				}
			}
		}
	}
}

