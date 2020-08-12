using System.Collections.Generic;
using System.Text.RegularExpressions;
using BotModel.Bots.BotTypes;

namespace Model.Logic.Coordinates.RegExp
{
	public static class DegreeDigital
	{
		private const string PatternDigital = @"-?\d{1,2}(\.|,)\d{2,16}";
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

		public static IEnumerable<Coordinate> GetCoordinates(string text)
		{
			foreach (Match match in Regex.Matches(text, DegreeDigital.Pattern))
			{
				var firstMinus = match.Groups[ConvertDegree.FirstMinus].Success ? -1 : 1;
				var secondMinus = match.Groups[ConvertDegree.SecondMinus].Success ? -1 : 1;

				yield return new Coordinate(
					ConvertDegree.Parse(match.Groups[DegreeDigital.FirstDigital].Value) * firstMinus,
					ConvertDegree.Parse(match.Groups[DegreeDigital.SecondDigital].Value) * secondMinus,
					match.Value.Trim());
			}
		}
	}
}