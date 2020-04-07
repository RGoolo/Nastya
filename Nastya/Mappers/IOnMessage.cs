using System.Collections.Generic;
using Model.Bots.BotTypes.Class;
using Model.Bots.BotTypes.Interfaces.Messages;

namespace Nastya.Mappers
{
	public interface IOnMessage
	{

		//ToDo IEnumerabe
		IEnumerable<TransactionCommandMessage> OnMessage(IBotMessage message);
	}

	public interface IMapper: IOnMessage
	{
		void AddInstance(object instance);
	}
}