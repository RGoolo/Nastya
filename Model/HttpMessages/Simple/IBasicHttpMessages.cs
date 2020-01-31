using System.Net;
using System.Threading.Tasks;

namespace Model.HttpMessages.Simple
{
	public interface IBasicHttpMessages
	{
		HttpWebRequest RequestGet(string url);
		HttpWebRequest RequestPost(string url, string context);

		Task<HttpWebResponse> Response(HttpWebRequest request);
		Task<string> GetText(HttpWebResponse response);
	}
}