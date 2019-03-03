
using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Web.Base;
using Web.Game.Model;
using Model.Logic.Settings;

namespace Web.DZR
{
	public class Dzr : BaseGame<Validator>
	{
		public Dzr(ISettings setting, TypeGame typeGame) : base(new Validator(setting), typeGame)
		{
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
			Encoding = Encoding.GetEncoding(1251);
		}

		protected override HttpWebRequest AddCustomHeaders(HttpWebRequest request)
		{
			//var username = Validator.Settings.GetValue(Const.Web.LoginAu);
			//var password = Validator.Settings.GetValue(Const.Web.PasswordAu); ;
			//var encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
			//request.Headers.Add("Authorization", "Basic " + encoded);

			request.Headers.Add("Referer", $@"http://classic.dzzzr.ru/{Validator.Settings.Web.Domen}/");
			request.Headers.Add("Cache-Control", @"max-age=0");
			request.Headers.Add("Origin", @"http://classic.dzzzr.ru");
			request.Headers.Add("Upgrade-Insecure-Requests", @"1");
			request.Headers.Add("User-Agent", @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/64.0.3282.186 Safari/537.36");
			request.Headers.Add("Accept", @"text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
			request.Headers.Add("DNT", @"1");
			request.Headers.Add("Accept-Encoding", @"gzip, deflate");
			request.Headers.Add("Accept-Language", @"ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7");
			return request;
		}

		protected override bool LogIn()
		{
			var request = GetWebRequest(Validator.LogInUrl());
			GetResponse(request);

			AddzrCoocies();

			var requestLogIn = PostHttpWebRequest(Validator.LogInUrl(), Validator.LogInContext()); //.GetResponse();

			//    requestLogIn.Headers.Add("Referer", @"http://classic.dzzzr.ru/spb/");
			//   requestLogIn.Headers.Add("Cache-Control", @"max-age=0");
			//      requestLogIn.Headers.Add("Origin", @"http://classic.dzzzr.ru");
			// requestLogIn.Headers.Add("Upgrade-Insecure-Requests", @"1");
			//requestLogIn.Headers.Add("User-Agent", @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/64.0.3282.186 Safari/537.36");
			//requestLogIn.Headers.Add("Accept", @"text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
			//requestLogIn.Headers.Add("DNT", @"1");
			//requestLogIn.Headers.Add("Accept-Encoding", @"gzip, deflate");
			// requestLogIn.Headers.Add("Accept-Language", @"ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7");

			GetResponse(requestLogIn);
			request = GetWebRequest(Validator.GetUrl());

			var respons = GetResponse(request);

			return !IsLogOut(respons);
		}

		protected override bool IsLogOut(HttpWebResponse response) => false;


		public override void SendCode(string str, Guid replaceMsg)
		{
			//if (!Validator.Settings.Game.Send)
			//	return;

			if (string.IsNullOrEmpty(str))
				return;

			if (str.StartsWith("."))
			{
				SetEvent(new SimpleEvent(EventTypes.SendCode, str.Substring(1), replaceMsg));
				return;
			}

			if (str.StartsWith("!"))
			{
				SetEvent(new SimpleEvent(EventTypes.SendSpoyeler, str.Substring(1), replaceMsg));
				return;
			}

			var msg = str.Split(" ")[0]?.ToLower();

			foreach (Match match in Regex.Matches(msg, @"[\ddr@дрp]+"))
			{
				if (msg != match.Value)
					return;

				foreach (Match match2 in Regex.Matches(msg, @"[\d]+"))
				{
					if (match2.Value == msg)
						return;
				}

				var prefix = Validator.Settings.Game.Prefix?.ToLower() ?? string.Empty;

				msg = msg.Replace("д", "d");
				msg = msg.Replace("р", "r");
				msg = msg.Replace("p", "r");
				msg = msg.Replace("@", "r");

				msg = msg.StartsWith(prefix) ? msg : (prefix + msg);

				SetEvent(new SimpleEvent(EventTypes.SendCode, msg, replaceMsg));

				//SendSimpleMsg(msg);

				return;
			}
		}
	}
}
