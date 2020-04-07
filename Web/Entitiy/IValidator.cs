using System.Collections.Generic;
using Model.Bots.BotTypes.Interfaces.Ids;
using Model.Bots.BotTypes.Interfaces.Messages;
using Model.Logic.Settings;

namespace Web.Entitiy
{
	public interface IController
	{
		// event SendMsgsSyncDel SendMsgs; //ToDo to interface
		event SendMsgDel SendMsg; //ToDo to interface
		ISettings Settings { get; }
		void LogIn();
		List<IEvent> GetCode(string str, IUser user, IMessageId replaceMsg);
		void SendEvent(IEvent iEvent);
		void Refresh();
	}
}
