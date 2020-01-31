using HtmlAgilityPack;

namespace Web.DL.PageTypes
{
	public class Sector
	{
		public Sector(HtmlNode sectorNode)
		{
			var text = sectorNode.InnerText;
			Name = text.Substring(0, text.IndexOf(':'));
			Answer = text.Substring(text.IndexOf(':') + 1);
			Accepted = sectorNode.SelectSingleNode("child::span[@class='color_correct']") != null;
		}

		public string Name { get; }
		public string Answer { get; }
		public bool Accepted { get; }

		public override string ToString()
		{
			return Accepted switch
			{
				true => $"✅<b>{Name}:{Answer}</b>" ,
				false => $"❌{Name}: - ",
			};
		}
	}
}