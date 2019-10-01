using System;
using System.Linq;
using HtmlAgilityPack;

namespace Web.DL
{
	public class Bonus
	{
		public string Name { get; }
		public string Title { get; }
		public string Text { get; }
		public bool IsReady { get; }

		public Bonus(string name, string text)
		{

			Name = name;
			Text = text;
			IsReady = !string.IsNullOrEmpty(text);
		}

		public Bonus(HtmlNode node)
		{
			Name = node?.InnerText;
			try
			{
				var nodeText = node.FirstChild.InnerText.Trim();
				// Бонус 1: Гриб

				var first = nodeText.IndexOf(':');
				if (first != -1)
				{
					Name = nodeText.Substring(0, first);
					Title = nodeText.Substring(first + 2);
				}
				else
				{
					Name = nodeText;
				}

				Text = node.ChildNodes?.Skip(1).FirstOrDefault()?.InnerText;
				IsReady = !string.IsNullOrEmpty(Text);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
			
		}
	}
}