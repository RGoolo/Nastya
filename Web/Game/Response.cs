using System;
using System.Collections.Generic;
using System.IO;
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
		}

		private HttpWebRequest AddCustomHeaders(HttpWebRequest request)
		{
			if (_dzr)
			{
				request.Headers.Add("Connection", "keep-alive");
				request.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
				request.Headers.Add("Upgrade-Insecure-Requests", "1");
				request.Headers.Add("Host", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/76.0.3809.132 Safari/537.36");
				request.Headers.Add("User-Agent", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3");
				request.Headers.Add("Referer", "http://classic.dzzzr.ru/demo/go/");
				request.Headers.Add("Accept-Encoding", "gzip, deflate");
				request.Headers.Add("Accept-Language", "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7");






			/*	request.Headers.Add("Host", "classic.dzzzr.ru");
				request.Headers.Add("Upgrade-Insecure-Requests", "1");
				request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/76.0.3809.132 Safari/537.36");
				*/ //request.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3");
				//request.Headers.Add("Referer", "http://classic.dzzzr.ru/demo/go/");
				//request.Headers.Add("Accept-Encoding", "gzip, deflate");
				//request.Headers.Add("Accept-Language", "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7");
			}

			return request;
		}

		public HttpWebRequest GetWebRequest(string url) => AddCustomHeaders(WebRequestHelper.GetWebRequest(url, _cookies));

		public HttpWebResponse GetNextResponse(HttpWebRequest request)
		{
			try
			{
				var resqSite = (HttpWebResponse)request.GetResponse();
				_cookies = WebRequestHelper.GetCookies(resqSite.Headers, request.CookieContainer);
				return resqSite;
			}
			catch
			{

			}
			return null;
		}

		public HttpWebResponse GetNextResponse(string url) => GetNextResponse(GetWebRequest(url));

		public HttpWebRequest PostHttpWebRequest(string url, string context) => AddCustomHeaders(WebRequestHelper.PostHttpWebRequest(url, context, _cookies));

		public void AddDzrCoocies()
		{
			try
			{
				_cookies.Add(new Cookie("b", "b", "/", ".dzzzr.ru"));//ToDo: Domen ".dzzzr.ru"
				_cookies.Add(new Cookie("hotlog", "1", "/", ".dzzzr.ru"));//ToDo: Domen ".dzzzr.ru"
				_cookies.Add(new Cookie("dozorRegistered","1", "/", ".dzzzr.ru"));
				_cookies.Add(new Cookie("dozorSiteSession", "11", "/", ".dzzzr.ru"));
			}

			catch {/* Console.WriteLine(ex.Message);*/ }; //ToDo
		}

		public string GetHtmlText(HttpWebResponse response) => new StreamReader(response.GetResponseStream(), Encoding).ReadToEnd();
	}
}
