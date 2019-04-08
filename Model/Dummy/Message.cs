using Model.Types.Enums;
using System;
using System.Collections.Generic;
using Model.Types.Interfaces;
using Model.Types.Class;

namespace Model.Dummy
{
	public class Message : IMessage
	{
		public Guid BotId { get; }
		public string Text { get; set; }
		public MessageType TypeMessage { get; }
		public Guid ChatId => new Guid("BBAF99E5-EF91-4FD4-BA25-4A111A071111");
		public List<IMessageCommand> MessageCommands { get; set; }
		public DummyUser _dUser { get; set; } = new DummyUser();
		public IUser User => _dUser;

		public Guid MessageId { get; }
		public IMessage ReplyToMessage => null;
	
		public IResource Resource { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public CommandMessage AnswerOn => null;

		public CommandMessage ReplyToCommandMessage => null;

		public Message(Guid botId, Guid? replayToMsgId = null, string text = null)
		{
			TypeMessage = MessageType.Text; 
			BotId = botId;
			Text = text;
			MessageId = replayToMsgId.GetValueOrDefault();
			FillCommands();
		}

		private void FillCommands()
		{
			var cc = new CreaterCommands(new string[] { "/","-"});
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

