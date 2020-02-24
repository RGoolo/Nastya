using System;
using System.Collections.Generic;
using Model.BotTypes.Class;
using Model.BotTypes.Class.Ids;
using Model.BotTypes.Enums;
using Model.BotTypes.Interfaces;
using Model.BotTypes.Interfaces.Messages;

namespace Model.Dummy
{
	public class Message : IBotMessage
	{
		public string Text { get; set; }
		public MessageType TypeMessage { get; }
		public IChatId ChatId => new ChatGuid("BBAF99E5-EF91-4FD4-BA25-4A111A071111");
		public List<IMessageCommand> MessageCommands { get; set; }
		public DummyUser _dUser { get; set; } = new DummyUser();
		public IUser User => _dUser;
		public IMessageId MessageId { get; }
		public IBotMessage ReplyToMessage => null;
	
		public IResource Resource { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public IMessageToBot AnswerOn => null;

		public IMessageToBot ReplyToCommandMessage { get;  }


		public Message(IBotId botId, IMessageId replayToMsgId = null, string text = null)
		{
			TypeMessage = MessageType.Text; 
			
			Text = text;
			MessageId = replayToMsgId;
			FillCommands();
		}

		public Message(IBotId botId, IMessageToBot message)
		{
			TypeMessage = MessageType.SystemMessage;
			
			ReplyToCommandMessage = message;
			//MessageId = replayToMsgId.GetValueOrDefault();

			MessageCommands = new List<IMessageCommand>();
		}

		private void FillCommands()
		{
			if (string.IsNullOrEmpty(Text))
				return;
			
			var cc = new CreatorCommands(new string[] { "/","-"});
			
			MessageCommands = cc.CreateCommands(Text, cc.GetCommands(Text));
		}
	}

	public class DummyUser : IUser
	{
		public string Display => "Пользователь ПК";
		public Guid Id => new Guid("{75C20E89-B048-4EF6-8731-922E6DE587BA}");
		public TypeUser Type => TypeUser.Admin | TypeUser.Developer;
	}
}

