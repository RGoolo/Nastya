using System.Collections.Generic;
using System.Text.RegularExpressions;
using BotModel.Bots.BotTypes;

namespace Model.Logic.Coordinates.RegExp
{
	public static class RegExPoints
	{
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
			if (text == null) yield break;
			var i = 'A';

			foreach (var coordinate in DegreeDigital.GetCoordinates(text))
			{
				coordinate.Alias = i++;
				yield return coordinate;
			}

			foreach (var coordinate in Degree.GetCoordinates(text))
			{
				coordinate.Alias = i++;
				yield return coordinate;
			}
		}
	}
}