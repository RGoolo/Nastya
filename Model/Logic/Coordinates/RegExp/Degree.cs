using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Model.Logic.Coordinates.RegExp
{
	public static class Degree
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


		public static IEnumerable<Coordinate> GetCoordinates(string text)
		{
			foreach (Match match in Regex.Matches(text, Degree.Pattern))
			{
				var firstCode = ConvertDegree.FromDegree(
					match.Groups[Degree.First.Degree].Value,
					match.Groups[Degree.First.Minutes].Value,
					match.Groups[Degree.First.Seconds].Value,
					match.Groups[Degree.First.Milliseconds].Value,
					match.Groups[ConvertDegree.FirstMinus].Success);

				var secondCode = ConvertDegree.FromDegree(
					match.Groups[Degree.Second.Degree].Value,
					match.Groups[Degree.Second.Minutes].Value,
					match.Groups[Degree.Second.Seconds].Value,
					match.Groups[Degree.Second.Milliseconds].Value,
					match.Groups[ConvertDegree.SecondMinus].Success);

				yield return new Coordinate(firstCode, secondCode, match.Value.Trim());
			}
		}

	
	}
}