using System.Collections.Generic;
using System.Threading.Tasks;
using Model.BotTypes;
using Model.BotTypes.Class;
using Model.BotTypes.Class.Reflection;
using Model.BotTypes.Enums;
using Model.BotTypes.Interfaces.Messages;
using Model.Logic.Settings;

namespace Nastya.Mappers
{
	public abstract class BaseMethodMapper : BaseMapper
	{
		protected ISendMessages _sMessages { get; }
		protected ISettings _settingHelper { get; }

		public BaseMethodMapper(ISendMessages sMessages, ISettings settingHelper)
		{
			_sMessages = sMessages;
			_settingHelper = settingHelper;
		}

		protected TypeResource CheckRecoursiveResource(IBotMessage msg)
		{
			if (msg == null)
				return TypeResource.None;

			if ((msg.TypeMessage & MessageType.WithResource) != 0)
			{
				switch (msg.TypeMessage)
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


		protected void AddParam(List<TransactionCommandMessage> list, object result, IBotMessage msg)
		{
			if (result == null) return;

			switch (result)
			{
				case Task t:
#pragma warning disable 4014
					SetInQ(t, msg);
#pragma warning restore 4014
					break;
				case TransactionCommandMessage res2:
					list.Add(res2);
					break;
				case IEnumerable<TransactionCommandMessage> res3:
					list.AddRange(res3);
					break;
				case IMessageToBot res4:
					list.Add(new TransactionCommandMessage(res4));
					break;
				case IEnumerable<IMessageToBot> res5:
					list.Add(new TransactionCommandMessage(res5));
					break;
				case string res6:
					list.Add(new TransactionCommandMessage(MessageToBot.GetTextMsg(res6)));
					break;
			}
		}

		protected async Task SetInQ(Task res, IBotMessage botMessage)
		{
			switch (res)
			{
				case Task<TransactionCommandMessage> res2:
					Send(await res2);
					break;
				case Task<IEnumerable<TransactionCommandMessage>> res3:
					SendMsg(await res3);
					break;
				case Task<List<TransactionCommandMessage>> res31:
					SendMsg(await res31);
					break;
				case Task<TransactionCommandMessage[]> res32:
					SendMsg(await res32);
					break;
				case Task<IMessageToBot> res4:
					Send(new TransactionCommandMessage(await res4));
					break;
				case Task<IEnumerable<IMessageToBot>> res5:
					Send(new TransactionCommandMessage(await res5));
					break;
				case Task<List<IMessageToBot>> res51:
					Send(new TransactionCommandMessage(await res51));
					break;
				case Task<IMessageToBot[]> res52:
					Send(new TransactionCommandMessage(await res52));
					break;
				case Task<string> res6:
					{
						var msg = MessageToBot.GetTextMsg(await res6);
						msg.OnIdMessage = botMessage.MessageId;
						Send(new TransactionCommandMessage(msg));
					}
					break;
			}
		}

		protected void Send(TransactionCommandMessage tMessage)
		{
			_sMessages.Send(tMessage);
		}

		protected void SendMsg(IEnumerable<TransactionCommandMessage> msgs)
		{
			foreach (var res in msgs)
			{
				Send(res);
			}
		}
	}
}