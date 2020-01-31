using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Model.BotTypes.Class;

namespace Model.TelegramBot
{
	public class TelegramHTML
	{
		public static string RemoveAllTag(string str) => Regex.Replace(str, "<[^>]+>", string.Empty);

		public static string RemoveTag(Texter text)
		{
			if (text == null)
				return null;

			if (!text.Html) return text.ToString();

			const string pattern = "(</[^abi][^>]*>)|(<[^abi/][^>]*>)|(</(\\w){2,}>)|(<a[^ ][^>]*>)|(<(b|i)[^>]+>)";
			var result = new Regex(pattern).Replace(text.ToString(), string.Empty);

			// var xDocument = XDocument.Parse(result); //Check couple tags
			return result;
		}
	}
}
