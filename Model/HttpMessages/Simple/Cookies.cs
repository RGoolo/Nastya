using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Model.Logger;

namespace Model.HttpMessages.Simple
{
	public class Cookies
	{
		public CookieContainer CookieContainer { get; } = new CookieContainer();
		private readonly ILogger _logger = Logger.Logger.CreateLogger(nameof(Cookies));

		public Cookies()
		{

		}

		public Cookies(IEnumerable<Cookie> cookies): this()
		{
			AddCookies(cookies);
		}

		public HttpWebRequest AddCookiesTo(HttpWebRequest response)
		{
			response.CookieContainer = CookieContainer;
			return response;
		}

		public HttpWebResponse AddCookies(HttpWebResponse response)
		{
			AddCookies(GetCookies(response.Headers, _logger));
			return response;
		}

		private void AddCookies(IEnumerable<Cookie> cookies)
		{
			if (cookies == null) return;

			try
			{
				foreach (var cookie in cookies)
					CookieContainer.Add(cookie);
			}
			catch (Exception ex)
			{
				_logger.Error(ex);
			}
		}


		private static List<Cookie> GetCookies(WebHeaderCollection headers, ILogger logger)
		{
			var result = new List<Cookie>();
			for (var i = 0; i < headers.Count; i++)
			{
				var name = headers.GetKey(i);
				var value = headers.Get(i);
				if (name != "Set-Cookie") continue;

				var split = value.Split(";", StringSplitOptions.RemoveEmptyEntries).Select(x => x.TrimStart()).ToArray();
				if (split.Length <= 0) continue;

				try
				{
					var val = split[0].Split("=");
					if (val.Length < 2 || val[0] == "" || val[1] == "")
						continue;

					var domain = NormalizeCookies(split.FirstOrDefault(x => x.StartsWith("domain=", StringComparison.OrdinalIgnoreCase))?.Split("=")[1]);
					var path = NormalizeCookies(split.FirstOrDefault(x => x.StartsWith("path=", StringComparison.OrdinalIgnoreCase))?.Split("=")[1] ?? @"/");

					result.Add(domain == null
						? new Cookie(val[0], val[1], path)
						: new Cookie(val[0], val[1], path, domain));
				}
				catch (Exception ex) { logger.Error(ex); };
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
	}
}