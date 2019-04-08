using System.Net;
using Web.Base;
using Web.Game.Model;
using Model.Logic.Settings;

namespace Web.DZRLite
{
	public class DZRLite : BaseGame<Validator>
	{
		public DZRLite(ISettings setting, TypeGame typeGame) : base(new Validator(setting), typeGame)
		{

		}

		protected override bool LogIn()
		{
			var request = GetWebRequest(Validator.GetUrl());
			var respons = GetResponse(request);
			// http://demo.en.cx/Login.aspx
			var requestLogIn = PostHttpWebRequest(Validator.LogInUrl(), Validator.LogInContext()); //.GetResponse();
			GetResponse(requestLogIn);

			request = GetWebRequest(Validator.GetUrl());
			respons = GetResponse(request);

			request = GetWebRequest(Validator.GetUrl());
			respons = GetResponse(request);
			return !IsLogOut(respons);
		}

		protected override bool IsLogOut(HttpWebResponse response)
		{
			return false;
		}

		protected override void ThreadSaveEvent(IEvent iEvent)
		{
			switch (iEvent.EventType)
			{
				// GetContextSetCode

				case EventTypes.GetLvlInfo:
					Validator.AfterSendCode("123", iEvent.User, "0", null);
					break;
				case EventTypes.Refresh:
					Refresh();
					break;

				case EventTypes.StartGame:
					Start();
					break;

				case EventTypes.StopGame:
					Stop();
					break;

				case EventTypes.GetSectors:
					Validator.AfterSendCode(iEvent.Text, iEvent.User, "1", null);
					break;

				case EventTypes.GetAllSectors:
					Validator.AfterSendCode(iEvent.Text, iEvent.User, "2", null);
					break;
			}
		}
	}
}