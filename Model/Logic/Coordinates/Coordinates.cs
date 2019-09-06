using Model.Logic.Google;
using Model.Types.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Model.Logic.Coordinates
{
	//ToDo: сложно получилось, надо бы разбить на 2 класса.
	public class CoordinatesFactory
	{
		public CoordinatesFactory(IFileWorker fileWorker, string googleToken)
		{
			FileWorker = fileWorker;
			FactoryMaps = new FactoryMaps(googleToken, fileWorker);
			//FactoryMaps = new FactoryMaps
		}

		public static string YandexUrl = @"https://yandex.ru/maps/?{1}mode=routes&rtext={0}&z=12";

		public string YandexName { get; set; } = "[Я]";
		public string GoogleName { get; set; } = "[G]";
		public string YandexPointNameMe { get; set; } = "[Y маршрут от меня]";
		public string GooglePointNameMe { get; set; } = "[G маршрут от меня]";
		public string YandexPointName { get; set; } = "[Y маршрут]";
		public string GooglePointName { get; set; } = "[G маршрут]";
		public IFileWorker FileWorker { get; }
		private FactoryMaps FactoryMaps { get; }

		public static string YandexCity = @"ll={0}&";
		private string GetUrlYa(string text, bool fromMe = false) =>
			City != null ? 
			string.Format(YandexUrl, (fromMe ? "~" : "") + text, string.Format(YandexCity, City)) 
			: string.Format(YandexUrl, text, string.Empty);
		public Coordinate City;

		public static string GetUrl(string link, string name) => $"<a href=\"{link}\">{name}</a>";

		private string GetUrlLink(Maps maps, string yaCoord, bool withInfo = false) => 
			(withInfo? GetPlace(yaCoord): string.Empty) 
			+ GetUrl(GetUrlYa(yaCoord), YandexName) + " " + GetUrl(maps.ToString(false), GoogleName);

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
			var coords = getCoords(text).ToList();
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

		public void GetPicture(string str, IFileToken file) => FactoryMaps.SaveImg(file, new Maps(GetCoords(str)));
		public void GetPictureText(string str, IFileToken file) => FactoryMaps.SaveImg(file, new Maps(_splitString(str)));

		public static IEnumerable<Coordinate> GetCoords(string text)
		{
			//ToDo: remake logic!
			foreach (var coord in GetDegreeCoords(text))
				yield return coord;
			foreach (var coord in GetDegreeMinSecCoords(text))
				yield return coord;
		}

		public static IEnumerable<Coordinate> GetDegreeCoords(string text)
		{
			const string patternDigital = @"-?\d{1,2}\.\d{2,16}";
			const string n = "(N|n|н|Н)";
			const string s = "(?<firstMinus>S|s)";
			const string e = "(E|e|Е|е)";
			const string w = "(?<secondMinus>W|w)";
			var kod1 = $"(?<firstCode>(?<firstDigital>{patternDigital})({n}|{s})?)";
			var kod2 = $"(?<secondCode>(?<secondDigital>{patternDigital})({e}|{w})?)";
			var pattern = @"(^|\D)" + kod1 + @"(\s|\.|,){1,3}" + kod2;


            foreach (Match match in Regex.Matches(text, pattern))
            {
                var firstMinus = match.Groups["firstMinus"].Success ? -1 : 1;
                var secondMinus = match.Groups["secondMinus"].Success ? -1 : 1;

                yield return new Coordinate(Parse(match.Groups["firstDigital"].Value) * firstMinus, Parse(match.Groups["secondDigital"].Value) * secondMinus, match.Value.Trim());
            }
    	}

        private static float Parse(string str) =>string.IsNullOrEmpty(str) ? 0 : float.Parse(str, NumberStyles.Float, CultureInfo.InvariantCulture); 


		public static IEnumerable<Coordinate> GetDegreeMinSecCoords(string text)
        {
            const string firstDegree = @"(?<firstDegree>\d{1,2})";
            const string firstMinutes = @"(?<firstMinutes>\d{1,2})";
            const string firstSeconds = @"(?<firstSeconds>\d{1,2})(\W?\.(?<firstMilliSeconds>\d{1,8})?)";

            const string secondDegree = @"(?<secondDegree>\d{1,2})";
            const string secondMinutes = @"(?<secondMinutes>\d{1,2})";
            const string secondSeconds = @"(?<secondSeconds>\d{1,2})(\W?\.(?<secondMilliSeconds>\d{1,8})?)";

            var firstDigitalCode = ($@"(?<firstMinus>-)?{firstDegree}\W{firstMinutes}\W{firstSeconds}\W?");
            var secondDigitalCode = ($@"(?<secondMinus>-)?{secondDegree}\W{secondMinutes}\W{secondSeconds}\W?");

            const string separator = @"(\s|,|\.){0,3}";
			const string n = "(N|n|н|Н)?";
            const string s = "(?<firstMinus>S|s)";
            const string e = "(E|e|Е|е)?";
			const string w = "(?<secondMinus>W|w)";

			var kod1 = $"({n}|{s})?{firstDigitalCode}({n}|{s})?";
			var kod2 = $"({e}|{w})?{secondDigitalCode}({e}|{w})?";
			var pattern = kod1 + separator + kod2;

			foreach (Match match in Regex.Matches(text, pattern))
            {
                var firstCode = FromDegree(match.Groups["firstDegree"].Value, match.Groups["firstMinutes"].Value,
                    match.Groups["firstSeconds"].Value, match.Groups["firstMilliSeconds"].Value, match.Groups["firstMinus"].Success);

                var secondCode = FromDegree(match.Groups["secondDegree"].Value, match.Groups["secondMinutes"].Value,
                    match.Groups["secondSeconds"].Value, match.Groups["secondMilliSeconds"].Value, match.Groups["secondMinus"].Success);

                yield return new Coordinate(firstCode, secondCode, match.Value.Trim());
			}
		}

        private static float FromDegree(string i, string j, string k, string l, bool minus) =>
            FromDegree(Parse(i), Parse(j), Parse(k) + Parse((string.IsNullOrEmpty(l) ? "" : $".{l}")), minus ? -1 : 1);

        private static float FromDegree(float i, float j, float k, int minus) => (i + j / 60 + k / 60 / 60) * (minus);
    }
}
