using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace Web.DZR.PageTypes
{
	public class Tasks : List<Task>
	{
		private HtmlNodeCollection nodes;

		public Tasks(HtmlNodeCollection nodes, string defauiltUrl)
		{
			List<HtmlNode> sendNodes = new List<HtmlNode>();

			foreach (var node in nodes)
			{
				// if (node.InnerText == "Последние три события игры команды")
				//	continue;

				sendNodes.Add(node);

				var atrr = node.Attributes;

				if (node.GetAttributeValue("class", "0") == "codeform")
				{
					Add(new Task(sendNodes, defauiltUrl));
					sendNodes = new List<HtmlNode>();
				}
			}
		}

		public Task Main(string name)
		{
			//Settings.Game.Level
			var task = this.FirstOrDefault(x => x.LvlNumber == name);
			return task ?? this.FirstOrDefault();
		}


	}
}
