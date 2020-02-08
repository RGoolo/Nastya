using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Model.HttpMessages.Simple;

namespace Model.HttpMessages
{
	public class HttpMessages : IHttpFullMessages
	{
		private readonly IBasicHttpMessages _messages;

		public HttpMessages(IBasicHttpMessages messages)
		{
			_messages = messages;
		}

		public Task Response(string url) => ResponseGet(url);
		public Task Response(string url, string context) => ResponsePost(url, context);


		public async Task<string> GetText(string url) => await _messages.GetText(await _messages.Response(_messages.RequestGet(url)));
		public async Task<string> GetText(string url, string context) => await _messages.GetText(await _messages.Response(_messages.RequestPost(url, context)));
		public async Task<HtmlNode> GetDocumentNode(string url)
		{
			var page = await GetText(url);
			var htmlDocument = new HtmlDocument();
			htmlDocument.LoadHtml(page);
			return htmlDocument.DocumentNode;
		}

		public Task<HttpWebResponse> ResponseGet(string url) =>  _messages.Response(_messages.RequestGet(url));
		public Task<HttpWebResponse> ResponsePost(string url, string context) => _messages.Response(_messages.RequestPost(url, context));
		
		public HttpWebRequest RequestGet(string url) => _messages.RequestGet(url);
		public HttpWebRequest RequestPost(string url, string context) => _messages.RequestPost(url, context);
		public Task<HttpWebResponse> Response(HttpWebRequest request) => _messages.Response(request);

		public Task<string> GetText(HttpWebResponse response) => _messages.GetText(response);
	}
}