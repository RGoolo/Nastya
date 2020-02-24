using System;
using Model.Logic.Coordinates;
using Model.Logic.Google;
using Model.Logic.Yandex;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Model.BotTypes.Class;
using Model.Logic.Coordinates.RegExp;
using Model.TelegramBot;

namespace Web.Base
{
	public static class WebHelper
	{
		public static string RemoveAllTag(string str)
		{
			return Regex.Replace(str, "<[^>]+>", string.Empty);
		}



		public static string RemoveSpaces(string str)
		{
			try
			{
				return Regex.Replace(str, "((\r)|(\n)){3,}", "\r\n");
			}
			catch
			{

			}

			return str;
		}



	/*	public static List<IMessageToBot> ReplaceTextOnPhoto(string html, string _defaulUri) 
		{
			var result = new List<IMessageToBot>();
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

			result.Add(MessageToBot.GetTextMsg(new Texter(buffText, true)));

			foreach (var coord in coords)
				result.Add(MessageToBot.GetCoordMsg(coord));
			

			result.AddRange(textTask.Item2.Where(x => x.TypeUrl == WebHelper.TypeUrl.Img).Select(x => WithUrls(x)));
			return result;
		}
		*/
		private static IMessageToBot WithUrls(LinkStruct url)
		{
			var sb = new StringBuilder();
			sb.Append(GetUrl(url.Name, url.Url));
			sb.Append("		");
			sb.Append(GetUrl("[G]", GoogleImgForMaps.GetSearchPhotoUrl(url.Url)));
			sb.Append("		");
			sb.Append(GetUrl("[Y]", YandexGeocoder.GetSearchPhotoUrl(url.Url)));

			return MessageToBot.GetPhototMsg( url.Url, new Texter(sb.ToString(), true));
		}

		public static string GetUrl(string link, string name) => $"<a href=\"{name}\">{link}</a>";
	}
}
