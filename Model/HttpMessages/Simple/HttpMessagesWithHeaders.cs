using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Model.HttpMessages.Simple
{
	public class HttpMessagesWithHeaders : IBasicHttpMessages
	{
		private readonly List<(string, string)> _customHeaders;
		private readonly IBasicHttpMessages _messages;

		public HttpMessagesWithHeaders(IBasicHttpMessages messages, List<(string, string)> customHeaders)
		{
			_messages = messages;
			_customHeaders = customHeaders;
		}

		public HttpWebRequest RequestGet(string url) => AddCustomHeaders(_messages.RequestGet(url));
		public HttpWebRequest RequestPost(string url, string context) => AddCustomHeaders(_messages.RequestPost(url, context));
		public Task<HttpWebResponse> Response(HttpWebRequest request) => _messages.Response(request);
		public Task<string> GetText(HttpWebResponse response) => _messages.GetText(response);

		private HttpWebRequest AddCustomHeaders(HttpWebRequest request)
		{
			foreach (var (name, value) in _customHeaders)
				request.Headers.Add(name, value);

			return request;
		}
	}
}