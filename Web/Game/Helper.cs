using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

//using WebDL.HTML;

namespace Web.Game
{
	public static class Helper
	{
		public static CookieContainer GetCookies(WebHeaderCollection headers, CookieContainer cookieContainer)
		{
			var result = cookieContainer ?? new CookieContainer();
			for (var i = 0; i < headers.Count; i++)
			{
				var name = headers.GetKey(i);
				var value = headers.Get(i);
				if (name != "Set-Cookie") continue;

				var split = value.Split(";", StringSplitOptions.RemoveEmptyEntries).Select(x => x.TrimStart()).ToArray();

				//Match match = Regex.Match(value, "(.+?)=(.+?);");
				if (split.Length <= 0) continue;
				try
				{

					var val = split[0].Split("=");
					if (val.Length < 2 || val[0] == "" || val[1] == "")
						continue;

					var domen = NormalizeCookies( split.FirstOrDefault(x => x.StartsWith("domain=", StringComparison.CurrentCultureIgnoreCase))?.Split("=")[1]);
					var path = NormalizeCookies( split.FirstOrDefault(x => x.StartsWith("path=", StringComparison.CurrentCultureIgnoreCase))?.Split("=")[1] ?? @"/");
						
				
					result.Add(domen == null
						? new Cookie(val[0], val[1], path)
						: new Cookie(val[0], val[1], path, domen));
				}
				catch (Exception ex) {/* Console.WriteLine(ex.Message);*/ }; //ToDo:
			}
			return result;
		}

		private static string NormalizeCookies(string cook)
		{
			//бага, когда приходит 2 секции set cookies
			var result = cook.Replace("%2E", ".");
			if (result.Contains(",")) result = result.Split(",").First();
			return result;
		}



		public static HttpWebRequest PostHttpWebRequest(string url, string context, CookieContainer cookies)
		{
			var request = (HttpWebRequest)WebRequest.Create(url);

			byte[] s = Encoding.UTF8.GetBytes(context);
			request.CookieContainer = cookies;
			request.ContentType = "application/x-www-form-urlencoded";
			request.Method = "POST";
			request.ContentLength = s.Length;
			request.CookieContainer = cookies;

			using (var stream = request.GetRequestStream())
				stream.Write(s, 0, s.Length);

			return request;
		}

		public static HttpWebRequest GetWebRequest(string url, CookieContainer cookies)
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.CookieContainer = cookies;
			request.ContentType = "application/x-www-form-urlencoded";
			request.Method = "GET";
			return request;
		}

		public static string GetHTML(HttpWebRequest request, ref CookieContainer cookies, Encoding encoding = null)
		{
			var response = GetResponse(request, ref cookies);
			return new StreamReader(response.GetResponseStream(), encoding ?? Encoding.UTF8).ReadToEnd();// Encoding.GetEncoding(1251)
		}

		public static HttpWebResponse GetResponse(HttpWebRequest request, ref CookieContainer cookies)
		{
			var reponce = (HttpWebResponse)request.GetResponse();
			cookies = GetCookies(reponce.Headers, request.CookieContainer);
			return reponce;
		}
	}
}
