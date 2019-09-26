using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Types.Class
{
	public class Texter
	{
		public bool Html { get; set; } = false;
		public string Text { get; set; }

		public Texter (string text, bool html = false)
		{
			Text = text;
			Html = html;
		}

		public override string ToString() => Text;
	}
}
