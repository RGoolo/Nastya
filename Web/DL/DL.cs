using System;
using System.Net;
using System.Text.RegularExpressions;
using Web.Base;
using Model.Logic.Settings;
using Web.Game.Model;
using Model.Types.Interfaces;

namespace Web.DL
{

	public class DLgame : BaseGame<Validator>
	{
		public DLgame(ISettings setting, TypeGame typeGame) : base(new Validator(setting), typeGame)
		{
		}

		protected override bool LogIn()
		{
			var request = GetWebRequest(Validator.GetUrl());
			var respons = GetResponse(request);

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
			if (response == null) return true;
			return (response.ResponseUri.ToString().StartsWith(Validator.LogInUrl()));
		}

		public override void SendCode(string str, IUser user, Guid replaceMsg)
		{
			if (string.IsNullOrEmpty(str))
				return;

			if (str.StartsWith("."))
				SetEvent(new SimpleEvent(EventTypes.SendCode, user, str.Substring(1), replaceMsg));

			if (str.Contains(" "))
				return;

			foreach (Match match in Regex.Matches(str, @"\w+"))
			{
				if (match.Value.Length != str.Length)
					return;

				foreach (Match match2 in Regex.Matches(str, @"\d+"))
				{
					if (match.Value.Length == match2.Value.Length)
						return;
					SetEvent(new SimpleEvent(EventTypes.SendCode, user, match.Value, replaceMsg));
				}
			}
		}
	}
}
