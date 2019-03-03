using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;

namespace Web.DZR
{
	public class Spoilers : List<Spoiler>
	{
		//public List<Spoiler> SpoilersList { get; } = new List<Spoiler>();

		public string GetPostForCode(string code) => 	this.FirstOrDefault(x => !x.IsComplited)?.GetPostForCode(code);	
		
		public Spoilers(HtmlNode node, string defaulUri)
		{
			var spoilers = node?.SelectNodes("div")?.Where(x => x.ChildNodes.Any(y => y.InnerHtml.Contains("<!--beginSpoilerText-->") || y.InnerHtml.Contains("<form "))).ToList();

			if (spoilers == null)
				return;

			foreach (var htmlspoiler in spoilers)
				Add(new Spoiler(htmlspoiler, defaulUri));

		}
	}
}