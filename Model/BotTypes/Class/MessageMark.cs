using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Model.BotTypes.Enums;
using Model.Files.FileTokens;
using Model.Logic.Coordinates;
using Model.Logic.Model;

namespace Model.BotTypes.Class
{

	public class TransactionCommandMessage : IEnumerable<IMessageToBot>
	{
		private readonly List<IMessageToBot> _messages;

		public TransactionCommandMessage(IMessageToBot message)
		{
			_messages = new List<IMessageToBot>{message};
		}

		public TransactionCommandMessage(List<IMessageToBot> messages)
		{
			_messages = messages;
		}

		public TransactionCommandMessage(IEnumerable<IMessageToBot> messages)
		{
			_messages = messages.ToList();	
		}

		public IEnumerator<IMessageToBot> GetEnumerator() => _messages.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => _messages.GetEnumerator();
		
	}

	public interface IMessageToBot
	{
		MessageType TypeMessage { get; }
		SystemType SystemType { get; }
		object SystemResource { get; }
		IMessageId OnIdMessage { get; set; }
		Texter Text { get; set; }
		Coordinate Coordinate { get; set; }
		IChatFileToken FileToken { get; set; }
		Notification Notification { get; set; }
		object NotificationObject { get; set; }
		IMessageId EditMsg { get; set; }
	}

	public class MessageToBot : IMessageToBot
	{
		public MessageType TypeMessage { get; }
		public SystemType SystemType { get; }
		public object SystemResource { get; }

		public IMessageId OnIdMessage { get; set; }
		public Texter Text { get; set; }
		public Coordinate Coordinate { get; set; }
		public IChatFileToken FileToken { get; set; }
		public Notification Notification { get; set; }
		public object NotificationObject { get; set; }
		public IMessageId EditMsg { get; set; }

		//public CommandMessage() { }

		protected MessageToBot(MessageType msgType)
		{
			TypeMessage = msgType;
		}

		protected MessageToBot(SystemType sysType, object systemResource) : this(MessageType.SystemMessage)
		{
			SystemType = sysType;
			SystemResource = systemResource;
		}

		public static IMessageToBot GetSystemMsg(object obj, SystemType systemType)
			=> new MessageToBot(systemType, obj);

		public static IMessageToBot GetTextMsg(string text)
			=> new MessageToBot(MessageType.Text) { Text = new Texter(text) };

		public static IMessageToBot GetTextMsg(Texter texter)
			=> new MessageToBot(MessageType.Text) { Text = texter };

		public static IMessageToBot GetEditMsg(string text)
			=> GetEditMsg(new Texter(text));

		public static IMessageToBot GetEditMsg(Texter texter)
			=> new MessageToBot(MessageType.Edit) { Text = texter };

		public static IMessageToBot GetPhototMsg(IChatFileToken token, Texter text)
			=> new MessageToBot(MessageType.Photo) { Text = text, FileToken = token };

		public static IMessageToBot GetPhototMsg(IChatFileToken token, string text)
			=> GetPhototMsg(token, new Texter(text));

		public static IMessageToBot GetHTMLPhototMsg(IChatFileToken token, string text)
			=> GetPhototMsg(token, new Texter(text, true));

		public static IMessageToBot GetPhototMsg(string url, Texter text)
			=> GetPhototMsg(new UrlChatFileToken(url), text);

		public static IMessageToBot GetVoiceMsg(string url, string name)
			=> new MessageToBot(MessageType.Voice) { Text = new Texter(name), FileToken = new UrlChatFileToken(url)};

		public static IMessageToBot GetDocumentMsg(IChatFileToken path, string text)
			=> new MessageToBot(MessageType.Document) { Text = new Texter(text) };

		public static IMessageToBot GetCoordMsg(Coordinate coord, string text = null)
			=> new MessageToBot(MessageType.Coordinates) { Text = new Texter(text), Coordinate = coord};

		public static IMessageToBot GetInfoMsg(string text) => GetTextMsg(text);
		public static IMessageToBot GetWarningMsg(string text) => GetInfoMsg(text);
		public static IMessageToBot GetErrorMsg(string text) => GetInfoMsg(text);
		public static IMessageToBot GetErrorMsg(ModelException ex) => GetErrorMsg(ex.Message);
		public static IMessageToBot GetErrorMsg(MessageException ex) 
		{
			var msg = GetErrorMsg((ModelException)ex);
			msg.OnIdMessage = ex.IMessage.MessageId;
			return msg;
		}	
	}
}
