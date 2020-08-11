using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Model.Bots.BotTypes.Class;
using Model.Bots.BotTypes.Enums;
using Model.Bots.BotTypes.Interfaces.Messages;
using Model.Logic.Model;
using Model.Logic.Settings;

namespace Nastya.Mappers
{
	public abstract class BaseMethodMapper : BaseMapper
	{
		protected ISendMessages _sMessages { get; }
		protected IChatService _settingHelper { get; }

		public BaseMethodMapper(ISendMessages sMessages, IChatService settingHelper)
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
				case IEnumerable<TransactionCommandMessage> res2:
					list.AddRange(res2);
					break;
				case IMessageToBot res2:
					list.Add(new TransactionCommandMessage(res2));
					break;
				case IEnumerable<IMessageToBot> res2:
					list.Add(new TransactionCommandMessage(res2));
					break;
				case string res2:
					list.Add(new TransactionCommandMessage(MessageToBot.GetTextMsg(res2)));
					break;
					default:
					throw new GameException("Не поддерживает возращаемый тип.");
				case null:
					break;
			}
		}

		protected async Task SetInQ(Task res, IBotMessage botMessage)
		{
			try
			{
				switch (res)
				{
					case Task<TransactionCommandMessage> res2:
						Send(await res2);
						break;
					case Task<IEnumerable<TransactionCommandMessage>> res2:
						SendMsg(await res2);
						break;
					case Task<List<TransactionCommandMessage>> res2:
						SendMsg(await res2);
						break;
					case Task<TransactionCommandMessage[]> res2:
						SendMsg(await res2);
						break;
					case Task<IMessageToBot> res2:
						Send(new TransactionCommandMessage(await res2));
						break;
					case Task<IEnumerable<IMessageToBot>> res2:
						Send(new TransactionCommandMessage(await res2));
						break;
					case Task<List<IMessageToBot>> res2:
						Send(new TransactionCommandMessage(await res2));
						break;
                    case Task<IList<IMessageToBot>> res2:
                        Send(new TransactionCommandMessage(await res2));
                        break;
					case Task<IMessageToBot[]> res2:
						Send(new TransactionCommandMessage(await res2));
						break;
					case Task<string> res2:
					{
						var msg = MessageToBot.GetTextMsg(await res2);
						msg.OnIdMessage = botMessage.MessageId;
						Send(new TransactionCommandMessage(msg));
					}
						break;
					case null:
						break;
					default:
						Send(new TransactionCommandMessage("Не поддерживает возращаемый тип."));
						break;
				}
			}
			catch(Exception ex)
			{
				Send(new TransactionCommandMessage(ex.Message));
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