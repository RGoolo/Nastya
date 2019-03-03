using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Web.Base
{
	public static class WebHelper
	{
		public static string RemoveTag(string str)
		{
			return Regex.Replace(str, "<[^>]+>", string.Empty);
		}

		public static string ReplaceSpace(string str, string replaceOn = " ")
		{
			return Regex.Replace(str, "(\t|\r|\n)+", replaceOn);
		}

		public static string RemoteTagToTelegram(string str)
		{
			/*var result = Regex.Replace(str, "<br>|<p>","\n");
			
			//лишняя операция, но не составить Regex что бы не удалял <\a> 
			result = result.Replace()*/
			return Regex.Replace(Regex.Replace(str.Replace("<br>", "\n").Replace("<p>", "\n").Replace("</a>", "<a>"), "<[^((img)|(a))][^>]*>", string.Empty), "(\t|\r|\n)+", "\n");
			//return null;
		}

		public static (string, List<LinkStruct>) RemoveImg(string str, bool urlToText = true, string defaultUri = "")
		{
			List<LinkStruct> result = new List<LinkStruct>();

			foreach (Match match in Regex.Matches(str, "<[(img)][^>]*>"))
			{
				var start = match.Value.LastIndexOf("/") + 1;
				var end = match.Value.IndexOf("\"", start);
				var startUrl = match.Value.LastIndexOf("src=\"") + "src=\"".Length;

				var name = match.Value.Substring(start, end - start);
				var uri = GetFullUrl(match.Value.Substring(startUrl, end - startUrl), defaultUri);
				str = str.Replace(match.Value, $" {name} ");
				result.Add(new ImgLinkStruct(uri, name));
			}

			foreach (Match match in Regex.Matches(str, "<a href=\"([^\"]+)[^>]+>([^<]*)<a>"))
			{
				str = str.Replace(match.Value, urlToText ? match.Groups[2].ToString() : ($"<a href=\"{GetFullUrl(match.Groups[1].ToString(), defaultUri)}\">{match.Groups[2]}</a>"));
				result.Add(new AHrefLinkStruct(GetFullUrl(match.Groups[1].ToString(), defaultUri), match.Groups[2].ToString()));
			}
			return (str, result);
		}

		public static string GetFullUrl(string url, string startUri)
		{
			if (url.StartsWith("http://") || startUri == string.Empty || url.StartsWith("https://"))
				return url;

			while (url.StartsWith("./"))
			{
				url = url.Substring(2);
			}

			while (url.StartsWith("../"))
			{
				var end = startUri.LastIndexOf("/");
				startUri = startUri.Remove(end);
				url = url.Substring(3);
			}

			if (startUri.EndsWith("/"))
				return startUri + url;

			else return startUri + "/" + url;
		}

		public enum TypeUrl
		{
			Img, AHref
		}

		public static string GetTestPage(string file)
		{
			using (var stream = new StreamReader(file, Encoding.GetEncoding(1251)))
				return stream.ReadToEnd();
		}
	}
}
