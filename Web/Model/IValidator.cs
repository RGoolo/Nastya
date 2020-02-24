using Model.Logic.Settings;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Model.BotTypes.Class;
using Model.BotTypes.Interfaces;
using Model.BotTypes.Interfaces.Messages;
using Web.Game.Model;

namespace Web.Base
{
	public interface IController
	{
		event SendMsgsSyncDel SendMsgs; //ToDo to interface
		ISettings Settings { get; }
		void LogIn();
		List<IEvent> GetCode(string str, IUser user, IMessageId replaceMsg);
		void SendEvent(IEvent iEvent);
		void Refresh();
	}
}
