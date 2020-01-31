using System.Collections.Generic;
using System.Net;
using System.Text;
using Model.HttpMessages;
using Model.HttpMessages.Simple;
using Web.HttpMessages.Simple;

namespace Web.HttpMessages
{
	public class HttpMessagesFactory : Model.HttpMessages.HttpMessagesFactory
	{
		public static IHttpFullMessages DeadlineThrowAuthorizationMessages() => new Model.HttpMessages.HttpMessages(Model.HttpMessages.HttpMessagesFactory.Messages().AddCookiesMode().AddDlThrowMode());

		public static IHttpFullMessages Dzzzr()
		{
			var customHeaders = new List<(string, string)>()
			{
				("Accept", "text/html, application/xhtml+xml, image/jxr, */*"),
				("Referer", "http://classic.dzzzr.ru/demo/go/"),
				("Accept-Language", "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7"),
				("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36 Edge/16.16299"),
				("Accept-Encoding", "gzip, deflate"),
				("Host", "classic.dzzzr.ru"),
				("Connection", "Keep-Alive"),
			};

			var startedCookies = new List<Cookie>()
			{
				new Cookie("b", "b", "/", ".dzzzr.ru"), //ToDo: Domen ".dzzzr.ru"
				new Cookie("hotlog", "1", "/", ".dzzzr.ru"), //ToDo: Domen ".dzzzr.ru"
				new Cookie("dozorRegistered", "1", "/", ".dzzzr.ru"),
				new Cookie("dozorSiteSession", "11", "/", ".dzzzr.ru"),
			};

			return new Model.HttpMessages.HttpMessages(Model.HttpMessages.HttpMessagesFactory.Messages(Encoding.GetEncoding(1251), DecompressionMethods.GZip).AddCustomHeaderMode(customHeaders).AddCookiesMode(startedCookies));
		}

	}
}