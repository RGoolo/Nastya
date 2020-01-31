using System.Net;
using System.Threading.Tasks;
using Model.HttpMessages.Simple;

namespace Model.HttpMessages
{
	public interface IHttpFullMessages : IBasicHttpMessages, IHttpMessages
	{
		Task<HttpWebResponse> ResponseGet(string url);
		Task<HttpWebResponse> ResponsePost(string url, string context);
	}

	public interface IHttpMessages
	{
		Task Response(string url);
		Task Response(string url, string context);
		Task<string> GetText(string url);
		Task<string> GetText(string url, string context);
	}
}