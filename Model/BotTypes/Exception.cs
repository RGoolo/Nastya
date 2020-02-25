using Model.BotTypes.Interfaces;
using Model.BotTypes.Interfaces.Messages;
using Model.Logic.Model;

namespace Model.BotTypes
{
	public class MessageException : ModelException
	{
		public IBotMessage IMessage { get; }

		public MessageException(IBotMessage msg, string s) : base(s)
		{
			IMessage = msg;
		}
	}

	public class ArgumentNeedException : ModelException
	{
		public string ArgumentName{ get; }
		public ArgumentNeedException(string arg) : base($"Не хватает параметра: {arg}.")
		{
			ArgumentName = arg;
		}
	}
}
