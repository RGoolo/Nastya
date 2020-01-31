using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Model.Logic.Coordinates
{
	public static class RegExPoints
	{   
		private static string FirstMinus => "firstMinus";
		private static string SecondMinus => "secondMinus";

		private static class DegreeDigital
		{
			private const string PatternDigital = @"-?\d{1,2}\.\d{2,16}";
			private const string N = "(N|n|н|Н)";
			private const string S = "(?<firstMinus>S|s)";
			private const string E = "(E|e|Е|е)";
			private const string W = "(?<secondMinus>W|w)";

			private const string Kod1 = "(?<firstCode>(?<firstDigital>" + PatternDigital + ")(" + N + "|" + S +
										")?)";

			private const string Kod2 = "(?<secondCode>(?<secondDigital>" + PatternDigital + ")(" + E + "|" + W +
										"})?)";

			public const string Pattern = @"(^|\D)" + Kod1 + @"(\s|\.|,){1,3}" + Kod2;
			public const string FirstDigital = "firstDigital";
			public const string SecondDigital = "secondDigital";
		}

		private static class Degree
		{
			public static readonly string Pattern;

			public static class First
			{
				public static string Degree = "firstDegree";
				public static string Minutes = "firstMinutes";
				public static string Seconds = "firstSeconds";
				public static string Milliseconds = "firstMilliseconds";
			}

			public static class Second
			{
				public static string Degree = "secondDegree";
				public static string Minutes = "secondMinutes";
				public static string Seconds = "secondSeconds";
				public static string Milliseconds = "secondMilliseconds";

			}

			static Degree()
			{
				const string firstDegree = @"(?<firstDegree>\d{1,2})";
				const string firstMinutes = @"(?<firstMinutes>\d{1,2})";
				const string firstSeconds = @"(?<firstSeconds>\d{1,2})(\W{0,2}\.(?<firstMilliseconds>\d{1,8})?)";

				const string secondDegree = @"(?<secondDegree>\d{1,2})";
				const string secondMinutes = @"(?<secondMinutes>\d{1,2})";
				const string secondSeconds = @"(?<secondSeconds>\d{1,2})(\W{0,2}\.(?<secondMilliseconds>\d{1,8})?)";

				var firstDigitalCode =
					($@"(?<firstMinus>-)?{firstDegree}\W{firstMinutes}\W{firstSeconds}\W{{0,2}}");
				var secondDigitalCode =
					($@"(?<secondMinus>-)?{secondDegree}\W{secondMinutes}\W{secondSeconds}\W{{0,2}}");

				const string separator = @"(\s|,|\.){0,3}";
				const string n = "(N|n|н|Н)?";
				const string s = "(?<firstMinus>S|s)";
				const string e = "(E|e|Е|е)?";
				const string w = "(?<secondMinus>W|w)";

				var kod1 = $"({n}|{s})?{firstDigitalCode}({n}|{s})?";
				var kod2 = $"({e}|{w})?{secondDigitalCode}({e}|{w})?";
				Pattern = kod1 + separator + kod2;
			}
		}

		public static IEnumerable<Place> GetPlaces(string text)
		{
			foreach (Match match in Regex.Matches(text, @"(^|\n|;)[\s]*(?<word>[^\n;]+)"))
			{
				var word = match.Groups["word"];
				if (!word.Success) continue;
				
				yield return new Place(word.Value);
			}
		}


		public static IEnumerable<Coordinate> GetCoords(string text)
		{
			foreach (var coordinate in GetDegreeCoords(text))
			{
				yield return coordinate;
			}
				
			foreach (var coordinate in GetDegreeMinSecCoords(text))
				yield return coordinate;
		}

		private static IEnumerable<Coordinate> GetDegreeCoords(string text)
		{
			foreach (Match match in Regex.Matches(text, DegreeDigital.Pattern))
			{
				var firstMinus = match.Groups[FirstMinus].Success ? -1 : 1;
				var secondMinus = match.Groups[SecondMinus].Success ? -1 : 1;

				yield return new Coordinate(
					Parse(match.Groups[DegreeDigital.FirstDigital].Value) * firstMinus,
					Parse(match.Groups[DegreeDigital.SecondDigital].Value) * secondMinus,
					match.Value.Trim());
			}
		}

		private static float Parse(string str) => string.IsNullOrEmpty(str)
			? 0
			: float.Parse(str, NumberStyles.Float, CultureInfo.InvariantCulture);

		private static IEnumerable<Coordinate> GetDegreeMinSecCoords(string text)
		{
			foreach (Match match in Regex.Matches(text, Degree.Pattern))
			{
				var firstCode = FromDegree(
					match.Groups[Degree.First.Degree].Value,
					match.Groups[Degree.First.Minutes].Value,
					match.Groups[Degree.First.Seconds].Value,
					match.Groups[Degree.First.Milliseconds].Value,
					match.Groups[FirstMinus].Success);

				var secondCode = FromDegree(
					match.Groups[Degree.Second.Degree].Value,
					match.Groups[Degree.Second.Minutes].Value,
					match.Groups[Degree.Second.Seconds].Value,
					match.Groups[Degree.Second.Milliseconds].Value,
					match.Groups[SecondMinus].Success);

				yield return new Coordinate(firstCode, secondCode, match.Value.Trim());
			}
		}

		private static float FromDegree(string i, string j, string k, string l, bool minus) =>
			FromDegree(Parse(i), Parse(j), Parse(k) + Parse((string.IsNullOrEmpty(l) ? "" : $".{l}")),
				minus ? -1 : 1);

		private static float FromDegree(float i, float j, float k, int minus) =>
			(i + j / 60 + k / 60 / 60) * (minus);
	}
}