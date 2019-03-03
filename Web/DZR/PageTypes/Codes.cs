using System.Collections.Generic;
using Web.Base;

namespace Web.DZR
{
	public class Codes : List<Code>
	{
		public string Name;

		public Codes(string s)
		{
			var text = s.Split(":");
			if (text.Length < 2)
				return;

			Name = text[0].Trim();
			var codes = s.Substring(text[0].Length + 1).Split(",");
			foreach (var code in codes)
				Add(new Code(WebHelper.RemoveTag(code).Trim(), code.Contains("span")));
		}
	}
}

