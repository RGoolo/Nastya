using Model.Logic.Model;
using Model.Logic.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Web.Game;

namespace Web.DL
{
	public class DlWebValidator
	{
		private ISettings _settings { get; }
		public Response Response { get; }

		public DlWebValidator(ISettings settings)
		{
			_settings = settings;
			Response = new Response();
		}

		private string Domen() => _settings.Web.Domen;
		private string Site() => $@"http://{Domen()}/";
		private string Login() => _settings.Game.Login;
		private string Password() => _settings.Game.Password;

		private string LogInContext() => $@"socialAssign=0&Login={Login()}&Password={Password()}&EnButton1=Sign+In&ddlNetwork=1";

		private string GetUrl()
		{
			var result = $@"{Site()}{_settings.Web.BodyRequest}/{_settings.Web.GameNumber}/";

			if (_settings.Game.Sturm)
			{
				var lvl = _settings.Game.Level;
				if (!string.IsNullOrEmpty(lvl))
					return result + "?level=" + lvl;
			}
			return result;
		}

		private string LogInUrl() => $@"{Site()}Login.aspx";

		private string GetContextSetCode(string code, DLPage page) =>
			$"LevelId={page?.LevelId}&LevelNumber={page?.LevelNumber}&LevelAction.Answer=" + code;

		public DLPage SendCode(string code, DLPage page) => GetNextPage(Response.PostHttpWebRequest(GetUrl(), GetContextSetCode(code, page)));

		public DLPage LogIn()
		{
			Response.GetNextResponse(GetUrl());

			var requestLogIn = Response.PostHttpWebRequest(LogInUrl(), LogInContext()); //.GetResponse();
			var page = GetNextPage(requestLogIn);

			if (page.Type == TypePage.ErrorAuthentication)
				return null;

			return GetNextPage();
		}

		private DLPage GetNextPage(HttpWebRequest request) => GetNextPage(Response.GetNextResponse(request));

		public DLPage GetNextPage() => PageConstructor.GetNewPage(GetPage(), GetPage);

		private DLPage GetNextPage(HttpWebResponse page) => PageConstructor.GetNewPage(page, GetPage);

		protected string GetPage(HttpWebResponse response) => Response.GetHtmlText(response);

		protected HttpWebResponse GetPage() => Response.GetNextResponse(GetUrl());

		public bool IsLogOut(DLPage page) => page == null || page.Type == TypePage.ErrorAuthentication || page.Type == TypePage.Unknown;
	}
}
