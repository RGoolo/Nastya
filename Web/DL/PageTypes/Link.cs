using HtmlAgilityPack;
using Web.Base;

namespace Web.DL
{
	public class Link : ILink
	{
		public string Name { get; }
		public string Url { get; }

		public Link(HtmlNode node)
		{
			if (node.Name != "a")
			{
				return;
			}
			Name = node.InnerText;
			Url = node.GetAttributeValue("href", "");
		}
	}
}
