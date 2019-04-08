using HtmlAgilityPack;
using Web.Base;

namespace Web.DZR
{
	public class Hint
	{
		public string Name;
		public string Text;

		public Hint(HtmlNode nodeTitle, HtmlNode node)
		{
			Name = nodeTitle.InnerText;

			//ToDo RegEx
			var begin1 = node.InnerHtml.IndexOf("<!--");
			var end1 = node.InnerHtml.IndexOf("-->", begin1) + 3;
			var begin2 = node.InnerHtml.IndexOf("<!--", end1);

			Text = (node.InnerHtml.Substring(end1, begin2 - end1)).Trim();
		}

		public bool IsEmpty()=> string.IsNullOrEmpty(Text) || Text == "---" || Text == "-" || Text == "нет";
	}
}
