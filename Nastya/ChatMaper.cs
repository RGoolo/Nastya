using Model.Types;
using Model.Types.Class;
using Model.Types.Enums;
using Model.Types.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Model.Logic.Model;
using Model.Logic.Settings;
using Model.Types.Attribute;
using Nastya.Commands;

namespace Nastya
{
	public class ChatMaper
	{
		List<object> _classes = new List<object>();
		private Dictionary<Guid, (string, IMessage)> _needAnswers = new Dictionary<Guid, (string, IMessage)>();

		private List<MaperMethodOnAllMsg> _methodsAllMsg = new List<MaperMethodOnAllMsg>();
		private Dictionary<string, List<MaperPropInfo>> _properties = new Dictionary<string, List<MaperPropInfo>>();
		private Dictionary<string, List<MaperMethodInfo>> _methods = new Dictionary<string, List<MaperMethodInfo>>();
		private List<object> _instances;

		public ConcurrentQueue<TransactionCommandMessage> SendMessages = new ConcurrentQueue<TransactionCommandMessage>();
		public PayManager _payManager;

		private TypeBot _typeBot;
		private ISettings _settingHelper;
		public Guid ChatId { get; }

		public ChatMaper(TypeBot typeBot, Guid chatId)
		{
			ChatId = chatId;
			_typeBot = typeBot;
			_settingHelper = SettingsHelper.GetSetting(chatId);
			_payManager = new PayManager(chatId);
			FillChat();
		}

		public List<TransactionCommandMessage> OnMessage(IMessage message)
		{
			var result = new List<TransactionCommandMessage>();

			bool contain = _needAnswers.ContainsKey(message.User.Id);
			if (_needAnswers.ContainsKey(message.User.Id))
			{
				if (message.MessageCommands != null && message.MessageCommands.Any())
				{
					_needAnswers.Remove(message.User.Id);
					contain = false;
				}
			}

			result.AddRange(InvokeMethodsOnMessage(message));

			if ( ( message.MessageCommands != null && message.MessageCommands.Any()) || contain)
			{
				result.AddRange(SetAttribute(message));
				result.AddRange(InvokeMethods(message));
			}

			return result;
		}

		private void FillChat()
		{
			_instances = new List<object>();
			foreach (var commandClass in GetAllClasses())
			{
				var instance = Activator.CreateInstance(commandClass);
				_instances.Add(instance);

				if (instance is BaseCommand baseCommand)
				{
					baseCommand.SendMsg += BaseCommand_SendMsg;
					baseCommand.TypeBot = _typeBot;
					baseCommand.ChatId = ChatId;
				}

				foreach (var methodInfo in instance.GetType().GetMethods().Where(x => x.GetCustomAttribute<CommandOnMsgAttribute>(true) != null))
					_methodsAllMsg.Add(new MaperMethodOnAllMsg(methodInfo, instance));

				foreach (var methodInfo in instance.GetType().GetMethods().Where(x => x.GetCustomAttribute<CommandAttribute>(true) != null))
				{
					var propsAttr = methodInfo.GetCustomAttribute<CommandAttribute>(true);
					var mmInfo = new List<MaperMethodInfo> {new MaperMethodInfo(methodInfo, instance)};

					if (_methods.ContainsKey(propsAttr.Alias.ToLower()))
						_methods[propsAttr.Alias.ToLower()].AddRange(mmInfo);
					else
						_methods.Add(propsAttr.Alias.ToLower(), mmInfo);
				}

				foreach (var propInfo in instance.GetType().GetProperties().Where(x => x.GetCustomAttribute<CommandAttribute>(true) != null))
				{
					var propsAttr = propInfo.GetCustomAttribute<CommandAttribute>(true);
					var pInfo = new List<MaperPropInfo> { new MaperPropInfo(propInfo, instance) };

					if (_properties.ContainsKey(propsAttr.Alias.ToLower()))
						_properties[propsAttr.Alias.ToLower()].AddRange(pInfo);
					else
						_properties.Add(propsAttr.Alias.ToLower(), pInfo);
				}
			}
			FillProperty();
		}

		private void FillProperty()
		{
			foreach (var props in _properties)
			{
				var value = _settingHelper.GetValue(props.Key);
				if (string.IsNullOrEmpty(value)) continue;

				try
				{
					props.Value.ForEach(x => x.SetValue(value));
				}
				catch
				{
					//ToDo: Log Error
				}
			}
		}

		private void BaseCommand_SendMsg(TransactionCommandMessage tMessage)
		{
			SendMessages.Enqueue(tMessage);
		}

		private List<Type> GetAllClasses() => Assembly.GetExecutingAssembly().GetTypes().Where(x => x.GetCustomAttributes<CommandClassAttribute>(true)?.Any() == true).ToList();

		private List<TransactionCommandMessage> InvokeMethodsOnMessage(IMessage msg)
		{
			var result = new List<TransactionCommandMessage>();

			foreach (var method in _methodsAllMsg)
			{
				if (!CheckUsage(method, msg))
					continue;

				if ((msg.TypeMessage & MessageType.WithResource) != MessageType.Undefined && msg.Resource == null)
				{
					result.Add(new TransactionCommandMessage(CommandMessage.GetSystemMsg(msg, SystemType.NeedResource)));
				}
				else
				{
					var res = method.Invoke(msg);
					if (res != null)
					{
						if (res is TransactionCommandMessage res2)
							result.Add(res2);

						if (res is IEnumerable<TransactionCommandMessage> res3)
							result.AddRange(res3);
					}
				}
			}
			return result;
		}

		private bool CheckUsage(MaperMemberInfo info, IMessage msg)
		{
			if (!info.СheckUsage(msg))
				return false;

			return true;
			//ToDo check pay;
			//if (_payManager.CheckPurchased(info.InstanceAttribute, msg.User) && _payManager.CheckPurchased(info, msg.User))
			//	return true;

			throw new MessageException(msg, "А это платно(");
		}
		private TypeResource CheckRecoursiveResource(IMessage msg)
		{
			if (msg == null)
				return TypeResource.None;

			if ((msg.TypeMessage & MessageType.WithResource) != 0)
			{
				switch(msg.TypeMessage)
				{
					case MessageType.Photo:
						return TypeResource.Photo;
					case MessageType.Video:
						return TypeResource.Video;
					case MessageType.Document:
						return TypeResource.Document;
					case MessageType.Voice:
						return TypeResource.Voice;
				}
			}
			return CheckRecoursiveResource(msg.ReplyToMessage);
		}

		private List<TransactionCommandMessage> InvokeMethods(IMessage msg)
		{
			var needResourceInEnquue = false;

			var result = new List<TransactionCommandMessage>();
			if (msg.MessageCommands == null)
				return result;

			foreach (var msgCommand in msg.MessageCommands)
			{
				if (!_methods.ContainsKey(msgCommand.Name))
					continue;

				foreach (var method in _methods[msgCommand.Name])
				{
					if (!CheckUsage(method, msg))
						continue;

					if (method.CommandAttribute.Resource != TypeResource.None && msg.Resource == null)
					{
						var resource = CheckRecoursiveResource(msg);
						if (resource == method.CommandAttribute.Resource)
						{
							if (!needResourceInEnquue)
								result.Add(new TransactionCommandMessage(CommandMessage.GetSystemMsg(msg, SystemType.NeedResource)));
							needResourceInEnquue = true;
						}
						else if (resource == TypeResource.None)
							result.Add(new TransactionCommandMessage(CommandMessage.GetInfoMsg("Необходим ресурс.")));
					}
					else
					{
						//if (method.CommandAttribute.Resource != TypeResource.None && msg.Resource != null)
						//	msgCommand.Values.Insert(0, msg.Resource.File);

						if (method.CommandAttribute.Resource != TypeResource.None && method.CommandAttribute.Resource != msg.Resource.Type)
							continue;

						var res = method.Invoke(msg, msgCommand);

						if (res != null)
						{
							if (res is TransactionCommandMessage res2)
								result.Add(res2);

							if (res is IEnumerable<TransactionCommandMessage> res3)
								result.AddRange(res3);

							if (res is CommandMessage res4)
								result.Add(new TransactionCommandMessage(res4));

							if (res is IEnumerable<CommandMessage> res5)
								result.Add(new TransactionCommandMessage(res5));

							if (res is string res6)
								result.Add(new TransactionCommandMessage(CommandMessage.GetTextMsg(res6)));
						}
					}
				}
			}
			return result;
		}

		private List<CommandMessage> SetAttribute(IMessage msg, IMessageCommand msgCommand)
		{
			var result = new List<CommandMessage>();
			if (!_properties.ContainsKey(msgCommand.Name))
				return result;

			foreach (var property in _properties[msgCommand.Name])
			{
				try
				{
					if (!CheckUsage(property, msg))
						return result;

					if (msgCommand.SetDefaultValue)
					{
						property.SetDefault();
						_settingHelper.SetValue(msgCommand.Name, msgCommand.Name);
						result.Add(CommandMessage.GetInfoMsg($"{msgCommand.Name} сброшено."));
						return result;
					}

					if (msgCommand.Values.Any())
					{
						property.SetValue(msgCommand.FirstValue);
						_settingHelper.SetValue(msgCommand.Name, msgCommand.FirstValue);
						result.Add(CommandMessage.GetInfoMsg($"{msgCommand.Name}={msgCommand.FirstValue}"));
						return result;
					}
				}

				catch (ModelException mEx)
				{
					var message = CommandMessage.GetErrorMsg(mEx);
					message.OnIdMessage = msg.MessageId;
					result.Add(message);
				}
			}
			return result;
		}

		private List<TransactionCommandMessage> SetAttribute(IMessage msg)
		{
			var result = new List<TransactionCommandMessage>();
			var cMessages = new List<CommandMessage>();

			if (msg.MessageCommands == null || !msg.MessageCommands.Any())
			{
				if (!_needAnswers.ContainsKey(msg.User.Id))
					return result;

				if (msg.TypeMessage != MessageType.Text)
					return result;

				var value = _needAnswers[msg.User.Id];
				cMessages.AddRange(SetAttribute(msg, new MessageCommand(value.Item1, msg.Text)));
			}
			else foreach (var msgCommand in msg.MessageCommands)
			{
				if (msgCommand.Values.Any())
					cMessages.AddRange(SetAttribute(msg, msgCommand));
				else if (msg.ReplyToMessage != null)
					cMessages.AddRange(SetAttribute(msg, new MessageCommand(msgCommand.Name, msg.ReplyToMessage.Text)));
				else if (_properties.ContainsKey(msgCommand.Name) && _needAnswers.TryAdd(msg.User.Id, (msgCommand.Name, msg)))
					//ToDo for other bot
					result.Add(new TransactionCommandMessage(CommandMessage.GetInfoMsg($"Введите значение {msgCommand.Name} в следующем сообщении.\n/{msgCommand.Name}_ что бы сбросить значение или /cancel для отмены.")));
			}
			if (cMessages.Any())
				result.Add(new TransactionCommandMessage(cMessages));

			return result;
		}

		public void Dispose()
		{
			foreach (var instance in _instances)
			{
				if (instance is BaseCommand baseCommand)
				{
					baseCommand.SendMsg -= BaseCommand_SendMsg;
				}
			}
		}
	}
}

