using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Web.DL.PageTypes
{
	public class SectorsCollection
	{
		public List<Sector> Sectors { get; } = new List<Sector>();
		public string CountSectors { get; }
		public string SectorsRemain { get; }

		public SectorsCollection(HtmlNode contentBlock)
		{

			var Headers = contentBlock.SelectNodes("h3");
			if (Headers == null) return;

			var regexSectorsTitle = new Regex(@"(На уровне|Level has) (\d+).+");
			var regexSectorsRemain = new Regex(@"(?s)^.+(осталось закрыть|left ot close) (\d+).+$");

			foreach (var headerNode in Headers)
			{
				var text = headerNode.InnerText;
				var matches = regexSectorsTitle.Matches(text);
				if (matches.Count == 0) continue;

				CountSectors = regexSectorsTitle.Replace(matches[0].Value, "$2");
				SectorsRemain = regexSectorsRemain.Replace(text, "$2");

				var currentNode = headerNode;
				do
				{
					currentNode = currentNode?.NextSibling;
				} while (currentNode == null || currentNode.GetAttributeValue("class", "") != "cols-wrapper");

				var sectorsNodes = currentNode.SelectNodes(".//p");

				foreach (var sectorNode in sectorsNodes)
					Sectors.Add(new Sector(sectorNode));
			}
		}

		public string GetText(bool isAll)
		{
			var sb = new StringBuilder();
			sb.AppendLine($"На уровне осталось закрыть: {SectorsRemain} из {CountSectors}");
			
			var sectors = Sectors.Where(x => (!x.Accepted || isAll));
			return sb.Append(string.Join(Environment.NewLine, sectors.Select(x => x.ToString()).ToArray())).ToString();
		}
	}
}