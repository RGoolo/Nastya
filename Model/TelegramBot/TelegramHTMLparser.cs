using Model.Types.Class;
using System.Text.RegularExpressions;

namespace Model.TelegramBot
{
	public class TelegramHTML
	{
		public static string RemoveAllTag(string str)
		{
			return Regex.Replace(str, "<[^>]+>", string.Empty);
		}

		public static string RemoteTag(Texter text)
		{
			if (!text.Html) return text.ToString();

			const string pattern = "(</[^abi][^>]*>)|(<[^abi/][^>]*>)|(</(\\w){2,}>)|(<a[^ ][^>]*>)|(<(b|i)[^>]+>)";
			return new Regex(pattern).Replace(text.ToString(), string.Empty);
		}
	}
}
