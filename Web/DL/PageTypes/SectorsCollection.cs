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
		private List<Sector> SectorsInternal { get; } = new List<Sector>();
		public IReadOnlyCollection<Sector> AllSectors => SectorsInternal;
		private List<Sector> AcceptedSectorsInternal { get; } = new List<Sector>();
		public IReadOnlyCollection<Sector> AcceptedSectors => AcceptedSectorsInternal;

		public string CountSectors { get; }
		public string SectorsRemainString { get; }

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
				SectorsRemainString = regexSectorsRemain.Replace(text, "$2");

				var currentNode = headerNode;
				do
				{
					currentNode = currentNode?.NextSibling;
				} while (currentNode == null || currentNode.GetAttributeValue("class", "") != "cols-wrapper");

				var sectorsNodes = currentNode.SelectNodes(".//p");

				foreach (var sectorNode in sectorsNodes)
				{
					var sector = new Sector(sectorNode);
					SectorsInternal.Add(sector);
					if (sector.Accepted)
						AcceptedSectorsInternal.Add(sector);
				}
			}
		}

		public string ToString(bool isAll)
		{
			var sb = new StringBuilder();
			sb.AppendLine($"На уровне осталось закрыть: {SectorsRemainString} из {CountSectors}");

			var sectors = isAll ? SectorsInternal : AcceptedSectors;
			return sb.Append(string.Join(Environment.NewLine, sectors.Select(x => x.ToString()).ToArray())).ToString();
		}
	}
}