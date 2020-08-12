using System.Net;
using System.Threading.Tasks;
using BotModel.HttpMessages.Simple;

namespace Web.DL
{
	public class DlThrowAuthorizationMessages : IBasicHttpMessages
	{
		private readonly IBasicHttpMessages _messages;

		public DlThrowAuthorizationMessages(IBasicHttpMessages messages)
		{
			_messages = messages;
		}

		public HttpWebRequest RequestGet(string url) => _messages.RequestGet(url);
		public HttpWebRequest RequestPost(string url, string context) => _messages.RequestPost(url, context);
		public async Task<HttpWebResponse> Response(HttpWebRequest request) => CheckFailedAuthorization(await _messages.Response(request));
		public Task<string> GetText(HttpWebResponse response) => _messages.GetText(response);

		private HttpWebResponse CheckFailedAuthorization(HttpWebResponse response)
		{
			/*if (response.ResponseUri.AbsolutePath.StartsWith("/login.aspx", StringComparison.InvariantCultureIgnoreCase))
				throw new AuthenticationException();*/ //ToDo:  
			return response;
		}
	}
}