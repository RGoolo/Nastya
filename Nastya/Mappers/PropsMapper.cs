using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Model.Bots.BotTypes.Attribute;
using Model.Bots.BotTypes.Class;
using Model.Bots.BotTypes.Class.Reflection;
using Model.Bots.BotTypes.Enums;
using Model.Bots.BotTypes.Interfaces.Messages;
using Model.Logic.Model;
using Model.Logic.Settings;

namespace Nastya.Mappers
{
	public class PropsMapper : BaseMapper, IOnMessage
	{
		private readonly Dictionary<Guid, (string, IBotMessage)> _needAnswers =
			new Dictionary<Guid, (string, IBotMessage)>();

		private readonly Dictionary<string, List<MapperPropInfo>> _properties =
			new Dictionary<string, List<MapperPropInfo>>();

		private ISendMessages sMessages;
		private readonly IChatService _settingHelper;

		public PropsMapper(ISendMessages sMessages, IChatService settingHelper)
		{
			this.sMessages = sMessages;
			_settingHelper = settingHelper;
		}

		public void AddInstance(object instance)
		{
			foreach (var propInfo in instance.GetType().GetProperties()
				.Where(x => x.GetCustomAttribute<CommandAttribute>(true) != null))
			{
				var propsAttr = propInfo.GetCustomAttribute<CommandAttribute>(true);
				var pInfo = new List<MapperPropInfo> {new MapperPropInfo(propInfo, instance)};

				if (_properties.ContainsKey(propsAttr.Alias.ToLower()))
					_properties[propsAttr.Alias.ToLower()].AddRange(pInfo);
				else
					_properties.Add(propsAttr.Alias.ToLower(), pInfo);
			}
		}

		public void FillProperty()
		{
			foreach (var (key, mapperPropInfos) in _properties)
			{
				var value = _settingHelper.GetValue(key);
				if (string.IsNullOrEmpty(value)) continue;

				try
				{
					mapperPropInfos.ForEach(x => x.SetValue(value));
				}
				catch
				{
					//ToDo: Log Error
				}
			}
		}

		private List<IMessageToBot> SetAttribute(IBotMessage msg, IMessageCommand msgCommand)
		{
			var result = new List<IMessageToBot>();
			if (!_properties.ContainsKey(msgCommand.Name))
				return result;

			foreach (var property in _properties[msgCommand.Name])
			{
				try
				{
					if (!CheckUsage(property, msg))
						continue;

					if (msgCommand.SetDefaultValue)
					{
						property.SetDefault();
						_settingHelper.SetValue(msgCommand.Name, msgCommand.Name);
						result.Add(MessageToBot.GetInfoMsg($"{msgCommand.Name} сброшено."));
						continue;
					}

					if (msgCommand.Values.Any())
					{
						property.SetValue(msgCommand.FirstValue);
						var value = property.IsGet ? property.Get.ToString() : msgCommand.FirstValue;
						
						_settingHelper.SetValue(msgCommand.Name, msgCommand.FirstValue);
						
						result.Add(property.IsPassword
							? MessageToBot.GetInfoMsg($"{msgCommand.Name}=*****")
							: MessageToBot.GetInfoMsg($"{msgCommand.Name}={value}"));
						continue;
					}
				}
				catch (ModelException mEx)
				{
					var message = MessageToBot.GetErrorMsg(mEx);
					message.OnIdMessage = msg.MessageId;
					result.Add(message);
				}
			}

			return result;
		}

		public IEnumerable<TransactionCommandMessage> OnMessage(IBotMessage message)
		{
			var result = new List<TransactionCommandMessage>();
			var cMessages = new List<IMessageToBot>();

			if (message.MessageCommands == null || !message.MessageCommands.Any())
			{
				if (!_needAnswers.ContainsKey(message.User.Id))
					return result;

				if (message.TypeMessage != MessageType.Text)
					return result;

				var value = _needAnswers[message.User.Id];
				_needAnswers.Remove(message.User.Id);
				cMessages.AddRange(SetAttribute(message, new MessageCommand(value.Item1, message.Text)));
			}
			else
				foreach (var msgCommand in message.MessageCommands)
				{
					if (msgCommand.Values.Any())
						cMessages.AddRange(SetAttribute(message, msgCommand));
					
					else if (message.ReplyToMessage != null)
						cMessages.AddRange(SetAttribute(message,
							new MessageCommand(msgCommand.Name, message.ReplyToMessage.Text)));
					
					else if (_properties.ContainsKey(msgCommand.Name) &&
						_needAnswers.TryAdd(message.User.Id, (msgCommand.Name, message)))
						//ToDo for other bot
						result.Add(new TransactionCommandMessage(MessageToBot.GetInfoMsg(
							$"Введите значение {msgCommand.Name} в следующем сообщении.\n/{msgCommand.Name}_ что бы сбросить значение или /cancel для отмены.")));
				}

			if (cMessages.Any())
				result.Add(new TransactionCommandMessage(cMessages));

			return result;
		}
	}
}