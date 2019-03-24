﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
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
			AddRange(codes.Select(code => new Code(WebHelper.RemoveTag(code).Trim(), code.Contains("span"))));
			
		}

		public string Text(bool onlyNotAccepted = false)
		{
			var sb = new StringBuilder();
			sb.Append(Name + ":\n");
			int i = 0;
			foreach (var code in this.Where(x => !onlyNotAccepted || !x.Accepted))
				sb.Append($"#{++i}:{code}  ");
			return sb.ToString();
		}
	}
}

