using System;
using System.Collections.Generic;
using System.Linq;
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

		public static bool CheckPaarTags(string text)
		{
			const string pattern = "(<(?<open>\\w+)[^>]*>)|(</(?<close>\\w+)[^>]*>)"; //|((?<close></\w+)[^>]*>)
			Stack<string> tags = new Stack<string>();

			foreach (Match tag in Regex.Matches(text, pattern))
			{
				var open = tag.Groups["open"];
				if (open != null && !string.IsNullOrEmpty(open.Value))
				{
					tags.Push(open.Value);
					continue;
				}

				var close = tag.Groups["close"];
				if (close != null && !string.IsNullOrEmpty(close.Value))
				{
					if (tags.Count == 0) return false;
					var buff = tags.Pop();
					if (buff != close.Value) return false;
				}
			}

			return tags.Count == 0;
		}
	}
}
