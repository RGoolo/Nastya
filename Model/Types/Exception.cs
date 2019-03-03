using Model.Types.Interfaces;
using Model.Logic.Model;

namespace Model.Types
{
	public class MessageException : ModelException
	{
		public IMessage IMessage;

		public MessageException(IMessage msg, string s) : base(s)
		{
			IMessage = msg;
		}
	}
}
