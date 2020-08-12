using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace Web.DZR.PageTypes
{
	public class Spoilers : List<Spoiler>
	{
		//public List<Spoiler> SpoilersList { get; } = new List<Spoiler>();

		public string GetPostForCode(string code) => this.FirstOrDefault(x => !x.IsCompleted)?.GetPostForCode(code);	
		
		public Spoilers(HtmlNode node)
		{
			var spoilers = node?.SelectNodes("div")?.Where(x => x.ChildNodes.Any(y => y.InnerHtml.Contains("<!--beginSpoilerText-->") || y.Name == "form")).ToList();

			if (spoilers == null)
				return;

			foreach (var htmlspoiler in spoilers)
				Add(new Spoiler(htmlspoiler));
		}

		public string Text()
		{
			var sb = new StringBuilder();
			foreach (var spoiler in this)
			{
				sb.Append(spoiler.IsCompleted
					? $"➕Спойлер разгадан:\n{spoiler.Text}\n"
					: $"➖Спойлер не разгадан.\n");
			}
			return sb.ToString();
		}
	}
}