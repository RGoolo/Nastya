using System.Collections.Generic;
using System.Net;

namespace Model.HttpMessages.Simple
{
	public static class HttpMessagesExtensions
	{
		public static IBasicHttpMessages AddCustomHeaderMode(this IBasicHttpMessages messages, List<(string, string)> customHeaders)
		{
			return new HttpMessagesWithHeaders(messages, customHeaders);
		}

		public static IBasicHttpMessages AddCookiesMode(this IBasicHttpMessages messages, IEnumerable<Cookie> startedCookies = null)
		{
			return new HttpMessagesWithCookies(messages, startedCookies);
		}
	}
}