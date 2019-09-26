using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Web.Base;
using Web.DL.PageTypes;

namespace Web.DL
{
	public static class PageConstructor
	{

		public static DLPage GetNewPage(HttpWebResponse webResponse, Func<HttpWebResponse, string> func )
		{
			var page = new DLPage();
			if (webResponse.ResponseUri.AbsolutePath.StartsWith("/login.aspx", StringComparison.InvariantCultureIgnoreCase))
			{
				page.Type = TypePage.ErrorAuthentication;
				return page;
			}

			var html = func(webResponse);

			var htmlDocument = new HtmlDocument();
			htmlDocument.LoadHtml(html.Trim());
			
			var document = htmlDocument.DocumentNode;
			var contentBlock = document.SelectSingleNode("//div[@class='content']");
	

			page.Type = PageType(contentBlock, htmlDocument.DocumentNode);

			if (page.Type != TypePage.InProcess)
				return page;

			page.LevelTitle = LevelTitle(contentBlock);
			page.TimeToEnd = TimeToEnd(document);
			page.Task = Task(document);
			page.CodeType = SetIsCodeReceived(document);
			page.LevelId = LevelId(document);
			page.LevelNumber = LevelNumber(document);
			page.Sectors = new SectorsCollection(contentBlock);
			page.Hints = Hints(document);
			page.Bonuses = Bonuses(document);

			return page;
		}

		private static TypePage PageType(HtmlNode contentBlock, HtmlNode document)
		{
			var error = document.SelectSingleNode("//div[@class='error']");
			if (error != null && error.InnerText == "Неправильный логин или пароль")
				return TypePage.ErrorAuthentication;
			
			//<div class="goback"><i></i><a href="/GameDetails.aspx?gid=64571">Вернуться</a></div>
			var goback = document.SelectSingleNode("//div[@class='goback']");
			if (goback != null && goback.InnerText == "Вернуться")
				return TypePage.Finished;

			if (contentBlock == null)
				return TypePage.Unknown;

			//< div class="error"><span id = "lblDBMessage" > Неправильный логин или пароль</span></div>

			//toDo other
			return TypePage.InProcess;
		}

		private static List<Link> Levels(HtmlNode documentNode) => documentNode.SelectNodes("//h3[@class='section level']/li/div/a[@class!='block']")
			?.Select(x => new Link(x)).ToList() ?? new List<Link>();

		private static DateTime? TimeToEnd(HtmlNode documentNode)
		{
			var firstTimer = documentNode.SelectSingleNode("//h3[@class='timer']")?.ParentNode;
			var regexTimer = new Regex("\"StartCounter\":(\\d+)");

			try
			{
				var match = regexTimer.Matches(firstTimer.InnerText).First() as Match;
				return new DateTime().AddSeconds(int.Parse(match.Groups[1].Value));
			}
			catch { };
			return null;
		}

		private static bool StringComaperStart(string strMain, string str1, string str2) => strMain.StartsWith(str1, StringComparison.InvariantCultureIgnoreCase) || strMain.StartsWith(str2, StringComparison.InvariantCultureIgnoreCase);

		private static Hint GetHint(HtmlNode hint)
		{
			if (hint.ChildNodes.Any())
			{
				var regexTimer = new Regex("\"StartCounter\":(\\d+)");

				try
				{
					var match = regexTimer.Matches(hint.InnerText).First() as Match;
					var timeToEnd = new DateTime().AddSeconds(int.Parse(match.Groups[1].Value));

					return new Hint(hint.FirstChild.InnerText, null, timeToEnd);
				}
				catch { };
			}
			return null;
		}

		private static List<Hint> Hints(HtmlNode documentNode)
		{
			var mainDiv = documentNode.SelectSingleNode("//h3[@class='color_bonus' or @class='color_correct']")?.ParentNode;
			if (mainDiv == null)
				return new List<Hint>();

			var hints = new List<Hint>();

			HtmlNode temp = null;
			foreach (var hint in mainDiv.ChildNodes)
			{
				if (temp == null)
				{
					if (StringComaperStart(hint.InnerText, "Подсказка", "Penalty"))
					{
						if (hint.Name == "h3")
							temp = hint;
						else if (hint.Name == "span")
						{
							var h = GetHint(hint);
							if (h != null)
								hints.Add(h);
						}
						else
							temp = null;
					}
					else
						temp = null;

					continue;
				}

				switch (hint.Name)
				{
					case "h3":
						{
							temp = hint.Name == "h3" && StringComaperStart(hint.InnerText, "Подсказка", "Penalty")
								? hint
								: null;
							break;
						}
					case "p":
						{
							hints.Add(new Hint(temp.InnerHtml, hint.InnerText, DateTime.MinValue));
							temp = null;
							break;
						}
				}
			}

			if (temp != null)
			{
				var text = temp.InnerHtml;
				hints.Add(new Hint(text, text, DateTime.MinValue));
			}

			return hints;
		}

		private static List<Bonus> Bonuses(HtmlNode documentNode)
		{
			//var bonuses = new ();

			//<h3 class="color_bonus"> 
			var mainDiv = documentNode.SelectSingleNode("//h3[@class='color_bonus' or @class='color_correct']")?.ParentNode;

			var bonusesNode = mainDiv?.ChildNodes.Where(x => x.Attributes.Any(y => (y.Value == "color_bonus" || y.Value == "color_correct") && y.Name == "class") || x.Name == "p");
			return bonusesNode?.Select(x => new Bonus(x.InnerHtml, null)).ToList() ?? new List<Bonus>();
		}

		private static TypeCode SetIsCodeReceived(HtmlNode documentNode)
		{
			//IsReceived = TypeCode.None;

			var row = documentNode.SelectNodes("//ul[@class='history']//li")?.FirstOrDefault();

			if (null == row)
				return TypeCode.None;

			if ("color_correct" == row.GetAttributeValue("class", ""))
				return TypeCode.Received;

			if ("incorrect" == row.GetAttributeValue("id", ""))
				return TypeCode.NotReceived;

			return TypeCode.None;
		}

		private static string LevelId(HtmlNode documentNode) => documentNode.SelectSingleNode("//input[@name='LevelId']")?.GetAttributeValue("value", "");

		private static string LevelNumber(HtmlNode documentNode) => documentNode.SelectSingleNode("//input[@name='LevelNumber']")?.GetAttributeValue("value", "");

		private static string LevelTitle(HtmlNode contentBlock) => contentBlock.SelectSingleNode("h2[1]")?.InnerText;

		private static string Task(HtmlNode documentNode)
		{
			var mainDiv = documentNode.SelectNodes("//h3")?.FirstOrDefault(x => x.InnerText == "Задание" || x.InnerText == "Task");

			if (mainDiv?.NextSibling?.NextSibling == null)
				return null;

			var sb = new StringBuilder();
			foreach (var node in mainDiv.NextSibling.NextSibling.ChildNodes.Where(x => x.Name != "script"))
				sb.Append(node.InnerHtml);

			return sb.ToString();
		}
	}
}
