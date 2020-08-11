using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Model.Bots.BotTypes.Attribute;
using Model.Bots.BotTypes.Class;
using Model.Bots.BotTypes.Class.Reflection;
using Model.Bots.BotTypes.Enums;
using Model.Bots.BotTypes.Interfaces.Messages;
using Model.Logic.Settings;

namespace Nastya.Mappers
{
	public class MethodsAllMapper : BaseMethodMapper, IOnMessage
	{
		private readonly List<MapperMethodOnAllMsg> _methodsAllMsg = new List<MapperMethodOnAllMsg>();
	
		public IEnumerable<TransactionCommandMessage> OnMessage(IBotMessage message)
		{
			var result = new List<TransactionCommandMessage>();

			foreach (var method in _methodsAllMsg)
			{
				if (!CheckUsage(method, message))
					continue;

				if ((message.TypeMessage & MessageType.WithResource) != MessageType.Undefined && message.Resource == null)
				{
					result.Add(new TransactionCommandMessage(MessageToBot.GetSystemMsg(message, SystemType.NeedResource)));
				}
				else
				{
					var res = method.Invoke(message);
					AddParam(result, res, message);
				}
			}
			return result;
		}

		public void AddInstance(object instance)
		{
			foreach (var methodInfo in instance.GetType().GetMethods().Where(x => x.GetCustomAttribute<CommandOnMsgAttribute>(true) != null))
				_methodsAllMsg.Add(new MapperMethodOnAllMsg(methodInfo, instance));
		}

		public MethodsAllMapper(ISendMessages sMessages, IChatService settingHelper) : base(sMessages, settingHelper)
		{
		}
	}
}