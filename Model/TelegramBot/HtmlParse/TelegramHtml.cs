using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Model.BotTypes.Class;

namespace Model.TelegramBot
{
	public class TelegramHtml
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

		public static string GetFullUrl(string url, string startUrl)
		{
			if (url.StartsWith("http://") || startUrl == string.Empty || url.StartsWith("https://"))
				return url;

			while (url.StartsWith("./"))
			{
				url = url.Substring(2);
			}

			while (url.StartsWith("../"))
			{
				var end = startUrl.LastIndexOf("/", StringComparison.OrdinalIgnoreCase);
				startUrl = startUrl.Remove(end);
				url = url.Substring(3);
			}

			if (startUrl.EndsWith("/"))
				return startUrl + url;

			return startUrl + "/" + url;
		}


		private static void Imgs(string str, List<LinkStruct> result, string defaultUrl)
		{
			const string patternLastWord = "[^\"]*(\\\\|/)(?<lastWord>\\w*\\.?\\w+)";
			const string patternOneWord = "(?<oneWord>\\w*\\.?\\w+)";
			var patternSrcWord = $"src=\"(?<src>({patternOneWord})|({patternLastWord}))\"";
			var patternImg = $"<img[^>]*{patternSrcWord}[^>]*>";

			foreach (Match match in Regex.Matches(str, patternImg))
			{
				var src = match.Groups["src"].Value;
				var name = match.Groups["lastWord"].Value;
				if (string.IsNullOrEmpty(name))
					name = match.Groups["oneWord"].Value;

				var uri = GetFullUrl(src, defaultUrl);
				var link = new ImgLinkStruct(uri, name, match.Value);

				// str = str.Replace(match.Value, link.ToHref());
				result.Add(link);
			}
		}

		private static void Hrefs(string str, List<LinkStruct> result, string defaultUrl)
		{
			foreach (Match match in Regex.Matches(str, "<a href=\"([^\"]+)[^>]+>([^<]*)<a>"))
				result.Add(new AHrefLinkStruct(GetFullUrl(match.Groups[1].Value, defaultUrl), match.Groups[2].Value, match.Value));
		}

		public static string ReplaceTagsToHref(string str, List<LinkStruct> links)
		{
			foreach (var link in links)
			{
				switch (link.TypeUrl)
				{
					case TypeUrl.AHref:
						if (string.IsNullOrEmpty(string.Empty))
							continue;
						
						str = str.Replace(link.OriginalTag, link.ToHref());
						break;
					case TypeUrl.Img:
						str = str.Replace(link.OriginalTag, link.ToHref());
						break;
					case TypeUrl.Sound:
						break; //ToDo
				}
			}

			return str;
		}


		public static List<LinkStruct> GetLinks(string str, string defaultUrl)
		{
			var result = new List<LinkStruct>();
			Voices(str, result, defaultUrl);
			Imgs(str, result, defaultUrl);
			Hrefs(str, result, defaultUrl);
			return result;
		}


		// ToDo to FullUrl, delete "Тег audio не поддерживается вашим браузером"
		private static void Voices(string str, List<LinkStruct> result, string defaultUrl)
		{
			//<audio controls=""><source src="http://d1.endata.cx/data/123.mp3" type="audio/mp3"><br>Тег audio не поддерживается вашим браузером. <a href="http://d1.endata.cx/data/123.mp3">Скачайте музыку</a></audio>
			foreach (Match match in Regex.Matches(str, "<audio[^>]*><source src=\"(?<link>[^\">]*/(?<fileName>[^\"]*))\"[^>]*>"))
			{
				if (match.Success)
				{
					result.Add(new SoundLinkStruct(match.Groups["link"].Value, match.Groups["fileName"].Value, match.Value));
				}
			}
		}
	}
}
