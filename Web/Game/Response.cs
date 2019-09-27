using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Web.Base;

namespace Web.Game
{
	public class Response
	{
		private readonly bool _dzr;
		private CookieContainer _cookies = new CookieContainer();
		public Encoding Encoding { get; set; } = Encoding.UTF8;

		public Response(bool dzr = false)
		{
			_dzr = dzr;
			if (dzr)
				AddDzrCoocies();
		}

		private HttpWebRequest AddCustomHeaders(HttpWebRequest request)
		{
			if (_dzr)
			{
				request.Headers.Add("Accept", "text/html, application/xhtml+xml, image/jxr, */*");
				request.Headers.Add("Referer", "http://classic.dzzzr.ru/demo/go/");
				request.Headers.Add("Accept-Language", "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7");
				request.Headers.Add("User-Agent","Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36 Edge/16.16299");
				request.Headers.Add("Accept-Encoding", "gzip, deflate");
				request.Headers.Add("Host", "classic.dzzzr.ru");
				request.Headers.Add("Connection", "Keep-Alive");
			}

			return request;
		}

		public HttpWebRequest GetWebRequest(string url) =>
			AddCustomHeaders(WebRequestHelper.GetWebRequest(url, _cookies, _dzr));

		public HttpWebResponse GetNextResponse(HttpWebRequest request)
		{
			try
			{

				var resqSite = (HttpWebResponse) request.GetResponse();
				_cookies = WebRequestHelper.GetCookies(resqSite.Headers, request.CookieContainer);

				return resqSite;
			}
			catch (Exception ex)
			{

				Console.WriteLine(ex.Message);
				Console.WriteLine(ex.StackTrace);
			}

			return null;
		}

		public HttpWebResponse GetNextResponse(string url) => GetNextResponse(GetWebRequest(url));

		public HttpWebRequest PostHttpWebRequest(string url, string context) =>
			AddCustomHeaders(WebRequestHelper.PostHttpWebRequest(url, context, _cookies, _dzr));

		public void AddDzrCoocies()
		{
			try
			{
				_cookies.Add(new Cookie("b", "b", "/", ".dzzzr.ru")); //ToDo: Domen ".dzzzr.ru"
				_cookies.Add(new Cookie("hotlog", "1", "/", ".dzzzr.ru")); //ToDo: Domen ".dzzzr.ru"
				_cookies.Add(new Cookie("dozorRegistered", "1", "/", ".dzzzr.ru"));
				_cookies.Add(new Cookie("dozorSiteSession", "11", "/", ".dzzzr.ru"));
			}

			catch(Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			//ToDo
		}

		public string GetHtmlText(HttpWebResponse response) =>
			new StreamReader(response.GetResponseStream(), Encoding).ReadToEnd();
	}
}
