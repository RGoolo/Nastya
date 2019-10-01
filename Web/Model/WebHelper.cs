using System;
using Model.Logic.Coordinates;
using Model.Logic.Google;
using Model.Logic.Settings;
using Model.Logic.Yandex;
using Model.Types.Class;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Web.Game.Model;

namespace Web.Base
{
	public static class WebHelper
	{
		public static string RemoveAllTag(string str)
		{
			return Regex.Replace(str, "<[^>]+>", string.Empty);
		}

		public static string ReplaceSpace(string str, string replaceOn = " ")
		{
			return Regex.Replace(str, "(\t|\r|\n)+", replaceOn);
		}

		[Obsolete]
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

			foreach (Match match in Regex.Matches(str, "<(img)[^>]*>"))
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

		public static List<CommandMessage> ReplaceTextOnPhoto(string html, string _defaulUri) 
		{
			var result = new List<CommandMessage>();
			var buffText = RemoteTagToTelegram(html);

			var textTask = RemoveImg(buffText, false, _defaulUri);

			buffText = textTask.Item1;

			//var coords = PointsFactory.GetCoo (buffText).ToList();

			//var cord = new PointsFactory(new SettingsPoints(), new NetworkCredential(string.Empty, SecurityEnvironment.GetPassword("google_map")).Password, SettingsHelper.FileWorker);
			var coords = RegExPoints.GetCoords(buffText);


			// foreach (var coord in coords)
			//  buffText = buffText.Replace(coord.OriginText, cord.GetUrlLink(coord));

			foreach (var img in textTask.Item2.Where(x => x.TypeUrl == WebHelper.TypeUrl.Img))
				buffText = buffText.Replace(img.Name, $"<a href=\"{img.Url}\">[{img.Name}]</a>");

			result.Add(CommandMessage.GetTextMsg(new Texter(buffText, true)));

			foreach (var coord in coords)
				result.Add(CommandMessage.GetCoordMsg(coord));
			

			result.AddRange(textTask.Item2.Where(x => x.TypeUrl == WebHelper.TypeUrl.Img).Select(x => WithUrls(x)));
			return result;
		}

		private static CommandMessage WithUrls(LinkStruct url)
		{
			var sb = new StringBuilder();
			sb.Append(GetUrl(url.Name, url.Url));
			sb.Append("		");
			sb.Append(GetUrl("[G]", FactoryMaps.GetSearchPhotoUrl(url.Url)));
			sb.Append("		");
			sb.Append(GetUrl("[Y]", YandexGeocoder.GetSearchPhotoUrl(url.Url)));

			return CommandMessage.GetPhototMsg( url.Url, new Texter(sb.ToString(), true));
		}

		public static string GetUrl(string link, string name) => $"<a href=\"{name}\">{link}</a>";
	}
}
