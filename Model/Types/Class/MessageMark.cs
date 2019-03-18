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
		public MessageType TypeMessage { get; protected set; }
		public Guid OnIdMessage { get; set; }

		public bool CheckedCoordsInText { get; set; }
		public string Text { get; set; }
		public Coordinate Coord { get; protected set; }

		public bool WithHtmlTags { get; set; }
		public IFileToken FileToken { get; protected set; }

		public object SystemResource { get; protected set; }
		public SystemType SystemType { get; protected set; }

		public CommandMessage() { }

		protected CommandMessage(bool b, IFileToken photoPath, string text)
		{
			TypeMessage = MessageType.Photo;
			FileToken = photoPath;
			Text = text;
		}

		protected CommandMessage(string text, bool withHtmlTags = false)
		{
			TypeMessage = MessageType.Text;
			Text = text;
			WithHtmlTags = withHtmlTags;
		}

		protected CommandMessage(Coordinate coord, string description = "")
		{
			TypeMessage = MessageType.Coordinates;
			Coord = coord;
			Text = description;
		}

		public static CommandMessage GetSystemMsg(object message, SystemType systemType)
		{
			return new CommandMessage()
			{
				TypeMessage = MessageType.SystemMessage,
				SystemResource = message,
				SystemType = systemType
			};
		}

		public static CommandMessage GetTextMsg(string text, bool withHtmlTags = false)
		{
			return new CommandMessage()
			{
				TypeMessage = MessageType.Text,
				Text = text,
				WithHtmlTags = withHtmlTags,
			};
		}

		public static CommandMessage GetPhototMsg(IFileToken photoPath, string text, bool withHtmlTags = false)
		{
			return new CommandMessage()
			{
				TypeMessage = MessageType.Photo,
				FileToken = photoPath,
				Text = text,
				WithHtmlTags = withHtmlTags,
			};
		}

		public static CommandMessage GetDocumentMsg(IFileToken path, string text)
		{
			return new CommandMessage()
			{
				TypeMessage = MessageType.Document,
				FileToken = path,
				Text = text,
			};
		}


		public static CommandMessage GetInfoMsg(string text)
		{
			var info = GetTextMsg(text);
			//info.IsAnswer = true;
			return info;
		}

		public static CommandMessage GetWarningMsg(string text) => GetInfoMsg(text);
		public static CommandMessage GetErrorMsg(string text) => GetInfoMsg(text);
		public static CommandMessage GetErrorMsg(ModelException ex) => GetErrorMsg(ex.Message);
		public static CommandMessage GetErrorMsg(MessageException ex) 
		{
			var msg = GetErrorMsg((ModelException)ex);
			msg.OnIdMessage = ex.IMessage.MessageId;
			return msg;
	}	
		public static CommandMessage GetCoordMsg(Coordinate coord, string description = "", bool withHtml = false)
		{
			return new CommandMessage()
			{
				TypeMessage = MessageType.Coordinates,
				Coord = coord,
				Text = description,
			};
		}
	}

	public class  MessageMarks : CommandMessage
	{
		public IMessage Message { get; }

		private MessageMarks(IMessage msg)
		{
			Message = msg;
		}

		public static MessageMarks GetMessageMarks(IMessage msg, CommandMessage cmdMsg)
		{
			return new MessageMarks(msg)
			{
				TypeMessage = cmdMsg.TypeMessage,
				Text = cmdMsg.Text,
				Coord = cmdMsg.Coord,
				//IsAnswer = cmdMsg.IsAnswer,
			};
		}

		public static MessageMarks GetTextMark(IMessage msg, string text)
		{
			return new MessageMarks(msg)
			{
				TypeMessage = MessageType.Text,
				Text = text,
			};
		}

		public static MessageMarks GetCoordMark(IMessage msg, Coordinate coord, string text)
		{
			return new MessageMarks(msg)
			{
				TypeMessage = MessageType.Coordinates,
				Coord = coord,
				Text = text,
			};
		}
	}
}
