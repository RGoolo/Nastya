using Model.Types.Class;

namespace Nastya
{
	public interface ISendMessage
	{
		void SendMsg(TransactionCommandMessage tMessage);
	}
}