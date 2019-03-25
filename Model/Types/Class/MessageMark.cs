using System;
using System.Collections.Generic;
using System.Linq;
using Model.Logic.Coordinates;
using Model.Logic.Model;
using Model.Types.Enums;
using Model.Types.Interfaces;

namespace Model.Types.Class
{

	public class TransactionCommandMessage
	{
		public List<CommandMessage> Messages;
		public CommandMessage Message;
		public Guid ChatId { get; set; }
		public List<Guid> PrepareCommands { get; set; }

		public TransactionCommandMessage(string message)
		{
			Message = CommandMessage.GetTextMsg(message);
		}

		public TransactionCommandMessage(Texter message)
		{
			Message = CommandMessage.GetTextMsg(message);
		}

		public TransactionCommandMessage(CommandMessage message)
		{
			Message = message;
		}

		public TransactionCommandMessage(List<CommandMessage> messages)
		{
			Messages = messages;
		}

		public TransactionCommandMessage(IEnumerable<CommandMessage> messages)
		{
			Messages = messages.ToList();	
		}
	}

	public class CommandMessage
	{
		public MessageType TypeMessage { get; }
		public SystemType SystemType { get; }
		public object SystemResource { get; }

		public Guid OnIdMessage { get; set; }
		public Texter Texter { get; set; }
		public Coordinate Coord { get; set; }
		public IFileToken FileToken { get; set; }
		
		public Notification Notification { get; set; }

		//public CommandMessage() { }

		protected CommandMessage(MessageType msgType)
		{
			TypeMessage = msgType;
		}

		protected CommandMessage(SystemType sysType, object systemResource) : this(MessageType.SystemMessage)
		{
			SystemType = SystemType;
			SystemResource = systemResource;
		}


		public static CommandMessage GetSystemMsg(object obj, SystemType systemType)
			=> new CommandMessage(systemType, obj);

		public static CommandMessage GetTextMsg(string text)
			=> new CommandMessage(MessageType.Text) { Texter = new Texter(text)};

		public static CommandMessage GetTextMsg(Texter texter)
			=> new CommandMessage(MessageType.Text) { Texter = texter };

		public static CommandMessage GetPhototMsg(IFileToken token, Texter text)
			=> new CommandMessage(MessageType.Photo) { Texter = text };

		public static CommandMessage GetPhototMsg(IFileToken token, string text)
			=> GetPhototMsg(token, new Texter(text));

		public static CommandMessage GetPhototMsg(string url, Texter text)
			=> GetPhototMsg(new UrlFileToken(url), text); 

		public static CommandMessage GetDocumentMsg(IFileToken path, string text)
			=> new CommandMessage(MessageType.Document) { Texter = new Texter(text) };

		public static CommandMessage GetCoordMsg(Coordinate coord, string text = null)
			=> new CommandMessage(MessageType.Coordinates) { Texter = new Texter(text)};


		public static CommandMessage GetInfoMsg(string text) => GetTextMsg(text);
		public static CommandMessage GetWarningMsg(string text) => GetInfoMsg(text);
		public static CommandMessage GetErrorMsg(string text) => GetInfoMsg(text);
		public static CommandMessage GetErrorMsg(ModelException ex) => GetErrorMsg(ex.Message);
		public static CommandMessage GetErrorMsg(MessageException ex) 
		{
			var msg = GetErrorMsg((ModelException)ex);
			msg.OnIdMessage = ex.IMessage.MessageId;
			return msg;
		}	
	}
}
