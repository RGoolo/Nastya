using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Model.Logic.Model;
using Model.Logic.Settings;
using Model.Types.Interfaces;
using Web.DL;
using Web.Game;

namespace Web.DZR
{
	public class DzrWebValidator
	{
		private ISettings _settings { get; }
		public Response Response { get; }
		private string _url;
		private readonly string _tempDzrUrl =  new DzrUrl().ToString();
		public DzrWebValidator(ISettings settings)
		{
			_settings = settings;
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
			
			Response = new Response(true)
			{
				Encoding = Encoding.GetEncoding(1251)
			};

		}

		public DzrPage SendCode(string code, Task task) => GetNextPage(Response.PostHttpWebRequest(GetUrl(), GetContextSetCode(code, task)));

		public DzrPage LogIn()
		{
			var requestLogIn = Response.PostHttpWebRequest(LogInUrl(), LogInContext()); //.GetResponse();
			GetNextPage(requestLogIn);
			return GetNextPage();
		}

		private DzrPage GetNextPage(HttpWebRequest request) => GetNextPage(Response.GetNextResponse(request));

		private DzrPage GetNextPage(HttpWebResponse page) => new DzrPage(GetPage(Response.GetNextResponse(GetUrl())), GetUrl());

		public DzrPage GetNextPage() => new DzrPage(GetPage(GetPage()), GetUrl());

		protected string GetPage(HttpWebResponse response) => response == null? null : Response.GetHtmlText(response);

		protected HttpWebResponse GetPage() => Response.GetNextResponse(GetUrl());
	
		public bool IsLogOut(DzrPage page) => page == null || page.Type == PageType.NotFound;

		public string GetContextSetCode(string code, Task task) => task?.GetPostForCode(code);

		public string LogInContext() => $@"notags=&action=auth&login={_settings.Game.Login}&password={_settings.Game.Password}";

		public string GetUrl() => GetBaseUrl() + "?" + _tempDzrUrl;

		private string GetBaseUrl()
		{
			if (_url != null)
				return _url;

			if ((_settings.TypeGame & TypeGame.Dummy) == TypeGame.Dummy)
				return _settings.Web.Domen.Split('\\').SkipLast(1).Aggregate((x, y) => x + "\\" + y);

			_url = $@"http://{_settings.Web.Domen}/{_settings.Web.BodyRequest}/go/";

			return _url;
		}

		public string LogInUrl() => GetBaseUrl();
	}
}