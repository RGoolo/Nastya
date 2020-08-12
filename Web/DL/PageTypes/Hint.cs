using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Web.DL.PageTypes
{

	public class Hints : IEnumerable<Hint>
	{
		private List<Hint> hints = new List<Hint>();

		public Hint GetById(int number)
		{
			var index = number - 1;
			if (hints.Count < index)
			{
				var hintN = hints[number];
				if (hintN.Number == number)
					return hintN;
			}

			return hints.FirstOrDefault(h => h.Number == number);
		}

		public void Add(Hint hint) => hints.Add(hint);
		public bool IsEmpty => hints.Count == 0;

		public override string ToString() => string.Join("\n", hints);

		public IEnumerator<Hint> GetEnumerator() => hints.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

	public class Hint
	{
		public string Name { get; }
		public string Text { get; }
		public bool IsReady { get; }
		public TimeSpan TimeToEnd { get; }
		public int Number { get; }

		public Hint(string name, string text, TimeSpan timeTo)
		{
			Name = name?.Replace("&nbsp;", " ");
			Text = text;
			if (text != null)
			{
				if (!text.Contains('\n') && text.Contains('\r'))
					Text = text.Replace("\r", "\r\n");
				Text = text?.Replace("&nbsp;", " "); ;
			}			
			
			TimeToEnd = timeTo;
			IsReady = !string.IsNullOrEmpty(text);

			var regexNumber = new Regex("\\d+");

			try
			{
				var match = regexNumber.Matches(Name).First() as Match;
				Number = int.Parse(match.Value);
			}
			catch
			{
				Number = 0;
			}
		}

		public override string ToString() => IsReady
			? $"⏳{Name}: {Text}\n"
			: $"⏳{Name} откроется через: {TimeToEnd.ToString()}";
	}
}