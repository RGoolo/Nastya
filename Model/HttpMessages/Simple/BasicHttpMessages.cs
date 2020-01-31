using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Model.HttpMessages.Simple
{
	public class BasicHttpMessages : IBasicHttpMessages
	{
		private readonly DecompressionMethods _decompressionMethods;
		private readonly Encoding _encoding;

		public BasicHttpMessages(Encoding encoding = null, DecompressionMethods decompressionMethods = DecompressionMethods.None)
		{
			_encoding = encoding ?? Encoding.UTF8;
			_decompressionMethods = decompressionMethods;
		}

		public HttpWebRequest RequestGet(string url) => WebRequestGet(url, _decompressionMethods);
		public HttpWebRequest RequestPost(string url, string context) => WebRequestPost(url, context, _decompressionMethods);
		public async Task<HttpWebResponse>  Response(HttpWebRequest request) => (HttpWebResponse) (await request.GetResponseAsync());
		
		public Task<string> GetText(HttpWebResponse response) =>  new StreamReader(response.GetResponseStream(), _encoding).ReadToEndAsync();

		private static HttpWebRequest WebRequestPost(string url, string context, DecompressionMethods decompression)
		{
			var request = (HttpWebRequest)WebRequest.Create(url);

			request.AutomaticDecompression = decompression; 
			request.ContentType = "application/x-www-form-urlencoded";
			request.Method = "POST";

			var s = Encoding.UTF8.GetBytes(context);
			request.ContentLength = s.Length;
			using (var stream = request.GetRequestStream())
				stream.Write(s, 0, s.Length);

			return request;
		}

		private static HttpWebRequest WebRequestGet(string url, DecompressionMethods decompression)
		{
			var request = (HttpWebRequest)WebRequest.Create(url);

			request.AutomaticDecompression = decompression;
			request.ContentType = "application/x-www-form-urlencoded";
			request.Method = "GET";
			return request;
		}

	}
}