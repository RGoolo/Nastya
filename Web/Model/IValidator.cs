using Model.Logic.Settings;
using Model.Types.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Web.Game.Model;

namespace Web.Base
{
	public interface IController
	{
		event SendMsgsSyncDel SendMsgs;

		ISettings Settings { get; }

		void LogIn();
		bool IsLogOut();

		List<IEvent> GetCode(string str, IUser user, Guid replaceMsg);

		void SendEvent(IEvent iEvent);

		void Refresh();
		
	}
}
