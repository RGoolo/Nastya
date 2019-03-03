using System.Text.RegularExpressions;
using System.Linq;
using System;

namespace Web.DL
{
	public class Hint
	{
		public string Name;
		public string Text;
		public bool IsReady;
		public DateTime TimeTo;
		public int Number;
		public Hint(string name, string text, DateTime timeTo)
		{
			Name = name;
			Text = text;
			TimeTo = timeTo;
			IsReady = !string.IsNullOrEmpty(text);

			var regexNumber = new Regex("\\d+");

			try
			{
				var match = regexNumber.Matches(Name).First() as Match;
				Number = int.Parse(match.Value);
			} catch
			{
				Number = 0;
			}
		}
	}
}