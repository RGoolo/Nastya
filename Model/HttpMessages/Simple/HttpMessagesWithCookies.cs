using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Model.HttpMessages.Simple
{

	public class HttpMessagesWithCookies : IBasicHttpMessages
	{
		private readonly IBasicHttpMessages _simpleMessages;
		private readonly Cookies _cookies;
		
		public HttpMessagesWithCookies(IBasicHttpMessages simpleMessages, IEnumerable<Cookie> startedCookies = null)
		{
			_simpleMessages = simpleMessages;
			_cookies = new Cookies(startedCookies);
		}

		public HttpWebRequest RequestGet(string url) => _cookies.AddCookiesTo(_simpleMessages.RequestGet(url));
		public HttpWebRequest RequestPost(string url, string context) => _cookies.AddCookiesTo(_simpleMessages.RequestPost(url, context));
		public async Task<HttpWebResponse> Response(HttpWebRequest request) => _cookies.AddCookies(await _simpleMessages.Response(request));
		public Task<string> GetText(HttpWebResponse response) => _simpleMessages.GetText(response);
	}
}
