using Model.Logic.Google;
using Model.Types.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Model.Logic.Coordinates
{
	//ToDo: сложно получилось, надо бы разбить на 2 класса.
	public class Coordinates
	{
		public Coordinates(IFileWorker fileWorker, string googleToken)
		{
			FileWorker = fileWorker;
			_factoryMaps = new FactoryMaps(googleToken, fileWorker);
			//FactoryMaps = new FactoryMaps
		}

		public static string yandexUrl = @"https://yandex.ru/maps/?{1}mode=routes&rtext={0}&z=12";

		public string YandexName { get; set; } = "[Я]";
		public string GoogleName { get; set; } = "[G]";
		public string YandexPointNameMe { get; set; } = "[Y маршрут от меня]";
		public string GooglePointNameMe { get; set; } = "[G маршрут от меня]";
		public string YandexPointName { get; set; } = "[Y маршрут]";
		public string GooglePointName { get; set; } = "[G маршрут]";
		public IFileWorker FileWorker { get; }
		private FactoryMaps _factoryMaps { get; }

		public static string yandexCity = @"ll={0}&";
		private string GetUrlYa(string text, bool fromMe = false) =>
			City != null ? 
			string.Format(yandexUrl, (fromMe ? "~" : "") + text, string.Format(yandexCity, City)) 
			: string.Format(yandexUrl, text, string.Empty);
		public Coordinate City;

		public static string GetUrl(string link, string name) => $"<a href=\"{link}\">{name}</a>";

		private string GetUrlLink(Maps maps, string YaCoord, bool withInfo = false) => 
			(withInfo? GetPlace(YaCoord): string.Empty) 
			+ GetUrl(GetUrlYa(YaCoord), YandexName) + " " + GetUrl(maps.ToString(false), GoogleName);

		public string GetUrlLink(Coordinate coord, bool withInfo) => GetUrlLink(FactoryMaps.GetMap(coord), $@"{coord.Latitude},{coord.Longitude}", withInfo);
		public string GetUrlLink(string coord, bool withInfo) => GetUrlLink(FactoryMaps.GetMap(coord), coord, withInfo);
		public string GetUrlLink(Coordinate coord) => GetUrlLink(coord, false);
		public string GetUrlLink(string coord) => GetUrlLink(coord, false);

		private string GetPoints(string maps, string yaPoint, bool fromMe = true) => GetUrl(maps.ToString(), fromMe ? GooglePointNameMe : GooglePointName) + " " + GetUrl(GetUrlYa(yaPoint, fromMe), fromMe ? YandexPointNameMe : YandexPointName);
		private string GetPoints(string maps, IEnumerable<string> yaPoints, bool fromMe = true) => GetPoints(maps, yaPoints.Aggregate((x, y) => x + "~" + y), fromMe);

		public string GetPlace(string str) => Yandex.YandexGeocoder.Geocode(str)?.FirstOrDefault()?.GeocoderMetaData.Text ?? string.Empty + " ";
		public Coordinate GetCoord(string str)
		{
			var point = Yandex.YandexGeocoder.Geocode(str)?.FirstOrDefault()?.Point;
			return point == null ? null : new Coordinate(point.Value, str); ;
		}
		
		public IEnumerable<Coordinate> GetTextCoord(string t)
		{
			var result = new List<Coordinate>();
			var split = t.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
			if (split != null)
				result.AddRange(split.Select(GetCoord));
			return result;
		}

		private readonly Func<string, string[]> _splitString = x => x.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

		public string GetPointes(string str)
		{
			return GetPointes(str,
				GetCoords,
				FactoryMaps.GetMap,
				(x) => x.OriginText);
		}

		public string GetTextPointes(string str)
		{
			return GetPointes(str,
				_splitString,
				FactoryMaps.GetMap,
				(x) => x);
		}

		private string GetPointes<T>(string text, 
			Func<string, IEnumerable<T>> getCoords, 
			Func<IEnumerable<T>, Maps> createMap, 
			Func<T, string> toString )
		{
			List<T> coords = getCoords(text).ToList();
			if (!coords.Any())
				return text;
			var map = createMap(coords);
			return GetPoints(map.ToString(false), coords.Select(toString)) + Environment.NewLine + GetPoints(map.ToString(true), coords.Select(toString), false);
		}

		public string ReplaceCoords<T>(string text,
			Func<string, IEnumerable<T>> getCoords,
			Func<IEnumerable<T>, Maps> createMap,
			Func<T, string> toString,
			Func<T, string> getUrlsLink
			)
		{
			var dic = new Dictionary<string, string>();
			var coords = getCoords(text);
			if (!coords.Any())
				return text;

			coords.ToList().ForEach(x => dic.TryAdd(toString(x), getUrlsLink(x)));

			if (coords.Count() > 1)
			{
				var maps = createMap(coords);
				dic.ToList().ForEach(x => text = text.Replace(x.Key, x.Key + x.Value));
				var yaPoints = coords.Select(toString);
				text += Environment.NewLine + GetPoints(maps.ToString(false), yaPoints);
				text += Environment.NewLine + GetPoints(maps.ToString(true), yaPoints, false);
			}
			return text;
		}

		public string ReplaceCoords(string str)
		{
			return ReplaceCoords(str,
				GetCoords,
				FactoryMaps.GetMap,
				(x) => x.OriginText,
				GetUrlLink);
		}

		public string ReplaceTextCoords(string str)
		{
			return ReplaceCoords(str, _splitString, FactoryMaps.GetMap, (x) => x, GetUrlLink);
		}

		public void GetPicture(string str, IFileToken file) => _factoryMaps.SaveImg(file, new Maps(GetCoords(str)));
		public void GetPictureText(string str, IFileToken file) => _factoryMaps.SaveImg(file, new Maps(_splitString(str)));

		public static IEnumerable<Coordinate> GetCoords(string text)
		{
			//ToDo: remake logic!
			foreach (var coord in GetGradusCoords(text))
				yield return coord;
			foreach (var coord in GetGradusMinSecCoords(text))
				yield return coord;
		}

		public static IEnumerable<Coordinate> GetGradusCoords(string text)
		{
			var patternСoord = @"-?\d{1,2}\.\d{2,16}";
			var N = "(N|n|н|Н)";
			var S = "(S|s)";
			var E = "(E|e|Е|е)";
			var W = "(W|w)";
			var kod1 = $"{patternСoord}({N}|{S})?";
			var kod2 = $"{patternСoord}({E}|{W})?";
			var kod = patternСoord + $"({N}|{S}|{E}|{W})?";
			var pattern = @"(\D|^)[^\.]" + kod1 + @"(\s|\.|,|;){1,3}" + kod2; //+ @"(\D|$)";

			var lst = new List<string>();

			//чет сложно, можно сделать лучше
			foreach (Match match in Regex.Matches(text, pattern))
			{
				lst.Add(match.Value);
				foreach (Match matchСoord in Regex.Matches(match.Value, kod))
					lst.Add(matchСoord.Value);
			}

			var i = 1;
			while (i < lst.Count())
			{
				if (lst.Count() > 1)
				{
					var j = (lst[i].Contains("E") || lst[i].Contains("e")) ? -1 : 1;
					var k = (lst[i + 1].Contains("S") || lst[i + 1].Contains("s")) ? -1 : 1;
					if (lst[i].Last() > '9') lst[i] = lst[i].Remove(lst[i].Length - 1);
					if (lst[i + 1].Last() > '9') lst[i + 1] = lst[i + 1].Remove(lst[i + 1].Length - 1);
					yield return new Coordinate(float.Parse(lst[i]) * j, float.Parse(lst[i + 1]) * k, lst[i - 1].Trim());
				}
				i += 3;
			}
		}

		public static IEnumerable<Coordinate> GetGradusMinSecCoords(string text)
		{
			string kod = (@"-?\d{1,2}[^A-Za-z0-9_]\d{1,2}[^A-Za-z0-9_]\d{1,2}(\.\d{0,6})?[^A-Za-z0-9_]");
			string separator = "( ,|\n)*";
			string N = "(N|n|н|Н)?";
			string S = "(S|s)";
			string E = "(E|e|Е|е)?";
			string W = "(W|w)";

			string kod1 = $"{kod}({N}|{S})?";
			string kod2 = $"{kod}({E}|{W})?";
			string pattern = kod1 + separator + kod2;

			float[] dig = new float[6];
			int i = 0;
			foreach (Match match in Regex.Matches(text, pattern))
			{

				foreach (Match match2 in Regex.Matches(match.Value, kod2))
				{
					foreach (Match match3 in Regex.Matches(match2.Value, @"-?\d{1,2}(\.\d{0,8})?"))
					{
						if (i < 6)
							dig[i] = float.Parse(match3.Value);
						i++;
					}
				}
				if (match.Value.Contains("S") || match.Value.Contains("s"))
					dig[0] *= -1;
				if (match.Value.Contains("W") || match.Value.Contains("w"))
					dig[3] *= -1;
				yield return new Coordinate(FromDegree(dig[0], dig[1], dig[2]), FromDegree(dig[3], dig[4], dig[5]), match.Value.Trim());
			}
		}

		private static float FromDegree(float i, float j, float k)
		{
			return ((i > 0 ? i : (-1 * i)) + j / 60 + k / 60 / 60) * (i > 0 ? 1 : -1);
		}
	}
}
