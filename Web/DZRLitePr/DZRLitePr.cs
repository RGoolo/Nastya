using System.Net;
using System.Text;
using Web.Base;
using Model.Logic.Settings;

namespace Web.DZRLitePr
{
	public class DZRLitePr : BaseGame<Validator>
	{
		public DZRLitePr(ISettings setting, TypeGame typeGame) : base(new Validator(setting), typeGame)
		{
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
			Encoding = Encoding.GetEncoding(1251);
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

		/* protected override void ThreadSaveEvent(IEvent iEvent)
		 {
			 switch (iEvent.EventType)
			 {
				 // GetContextSetCode

				 case EventTypes.GetLvlInfo:
					 Validator.AfterSendCode("123","0",5 );
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
					 Validator.AfterSendCode(iEvent.Text, "1", 5);
					 break;

				 case EventTypes.GetAllSectors:
					 Validator.AfterSendCode(iEvent.Text, "2", 5);
					 break;
			 }
		 }*/
	}
}