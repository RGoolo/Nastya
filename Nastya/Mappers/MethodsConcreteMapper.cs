using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Model.BotTypes.Attribute;
using Model.BotTypes.Class;
using Model.BotTypes.Class.Reflection;
using Model.BotTypes.Enums;
using Model.BotTypes.Interfaces.Messages;
using Model.Logic.Settings;

namespace Nastya.Mappers
{
	class MethodsConcreteMapper : BaseMethodMapper, IOnMessage 
	{
		private readonly Dictionary<string, List<MapperMethodInfo>> _methods = new Dictionary<string, List<MapperMethodInfo>>();
		private readonly Dictionary<Guid, (string, IBotMessage)> _needAnswers =
			new Dictionary<Guid, (string, IBotMessage)>();

		private ISendMessages sMessages;
		private readonly ISettings _settingHelper;
		public MethodsConcreteMapper(ISendMessages sMessages, ISettings settingHelper) : base(sMessages, settingHelper)
		{
			this.sMessages = sMessages;
			_settingHelper = settingHelper;
		}

		public IEnumerable<TransactionCommandMessage> OnMessage(IBotMessage message)
		{
			if ((message.MessageCommands != null && message.MessageCommands.Any()))
			{
				return InvokeMethods(message);
			}

			return Enumerable.Empty<TransactionCommandMessage>();
		}

		private IEnumerable<TransactionCommandMessage> InvokeMethods(IBotMessage message)
		{
				var needResourceInEnquue = false;

				var result = new List<TransactionCommandMessage>();
				if (message.MessageCommands == null)
					return result;

				foreach (var msgCommand in message.MessageCommands)
				{
					if (!_methods.ContainsKey(msgCommand.Name))
						continue;

					foreach (var method in _methods[msgCommand.Name])
					{
						if (!CheckUsage(method, message))
							continue;

						if (method.CommandAttribute.Resource != TypeResource.None && message.Resource == null)
						{
							var resource = CheckRecoursiveResource(message);
							if (resource == method.CommandAttribute.Resource)
							{
								if (!needResourceInEnquue)
									// sMessages.DownloadMessage(message);
									result.Add(new TransactionCommandMessage(MessageToBot.GetSystemMsg(message, SystemType.NeedResource)));
								needResourceInEnquue = true;
							}
							else if (resource == TypeResource.None)
								result.Add(new TransactionCommandMessage(MessageToBot.GetInfoMsg("Необходим ресурс.")));
						}
						else
						{
							//if (method.CommandAttribute.Resource != TypeResource.None && msg.Resource != null)
							//	msgCommand.Values.Insert(0, msg.Resource.File);

							if (method.CommandAttribute.Resource != TypeResource.None && method.CommandAttribute.Resource != message.Resource.Type)
								continue;
							try
							{
								var res = method.Invoke(message, msgCommand);
								AddParam(result, res, message);
							}
							catch (Exception e)
							{
								Console.WriteLine(e);
								throw;
							}
						}
					}
				}
				return result;
		}

		public void AddInstance(object instance)
		{
			foreach (var methodInfo in instance.GetType().GetMethods().Where(x => x.GetCustomAttribute<CommandAttribute>(true) != null))
			{
				var propsAttr = methodInfo.GetCustomAttribute<CommandAttribute>(true);
				var mmInfo = new List<MapperMethodInfo> { new MapperMethodInfo(methodInfo, instance) };

				if (_methods.ContainsKey(propsAttr.Alias.ToLower()))
					_methods[propsAttr.Alias.ToLower()].AddRange(mmInfo);
				else
					_methods.Add(propsAttr.Alias.ToLower(), mmInfo);
			}
		}
	}
}
