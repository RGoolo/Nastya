using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Model.Logger;
using Web.Base;
using Web.DL.PageTypes;
using Web.Entitiy;

namespace Web.DL
{
	public static class PageConstructor
	{
		public static DLPage CreateNewPage(string html)
		{
			// var page = new DLPage();
			return FillPage(html);
		}

		public static DLPage GetNewPage(string html) => FillPage(html);

		private static DLPage FillPage(string html)
		{
			var page = new DLPage {Html = html};
			var htmlDocument = new HtmlDocument();
			htmlDocument.LoadHtml(html.Trim());
			
			var document = htmlDocument.DocumentNode;
			var contentBlock = document.SelectSingleNode("//div[@class='content']");
			
			page.Type = PageType(contentBlock, htmlDocument.DocumentNode);
			
			if (page.Type != TypePage.InProcess && page.Type != TypePage.NotStarted)
				return page;

			page.LevelTitle = LevelTitle(contentBlock);
			page.TimeToEnd = TimeToEnd(document, page.Type);
			page.Body = Task(document, page.Type);
			page.CodeType = SetIsCodeReceived(document);
			page.LevelId = LevelId(document);
			page.LevelNumber = LevelNumber(document);
			page.Sectors = new SectorsCollection(contentBlock);
			page.Hints = Hints(document);
			page.Bonuses = Bonuses(document);
			page.Levels = Levels(contentBlock);
			page.IsSturm = page.Levels.Any();

			return page;
		}

		private static TypePage PageType(HtmlNode contentBlock, HtmlNode document)
		{
			var error = document.SelectSingleNode("//div[@class='error']");
			if (error != null && error.InnerText == "Неправильный логин или пароль")
				return TypePage.ErrorAuthentication;
			
			var gameStarted = document.SelectSingleNode("//span[@id='Panel_TimerHolder']"); 
		
				//<div class="goback"><i></i><a href="/GameDetails.aspx?gid=64571">Вернуться</a></div>
			var goBack = document.SelectSingleNode("//div[@class='goback']");
			if (goBack != null && goBack.InnerText == "Вернуться")
			{
				return gameStarted != null && gameStarted.InnerText.Contains("Игра начнется через")
					? TypePage.NotStarted
					: TypePage.Finished;
			}

			//<center class="gameCongratulation">
			var gameCongratulation = document.SelectSingleNode("//center[@class='gameCongratulation']");
			if (gameCongratulation != null)
				return TypePage.Finished;
				
			if (contentBlock == null)
				return TypePage.Unknown;

			//< div class="error"><span id = "lblDBMessage" > Неправильный логин или пароль</span></div>

			//toDo other
			return TypePage.InProcess;
		}

		private static List<string> Levels(HtmlNode documentNode) => documentNode.SelectNodes("//ul[@class='section level']/li/i")
			?.Select(x => x.InnerText).ToList() ?? new List<string>();

		private static TimeSpan? TimeToEnd(HtmlNode documentNode, TypePage pageType)
		{
			var classTimer = (pageType == TypePage.NotStarted) ? "//span[@id='Panel_TimerHolder']" : "//h3[@class='timer']";
		
			var firstTimer =  documentNode.SelectSingleNode(classTimer)?.ParentNode;
			var regexTimer = new Regex("\"StartCounter\":(\\d+)");
			if (firstTimer == null) return null;

			try
			{
				var match = regexTimer.Matches(firstTimer.InnerHtml).First() as Match;
				return TimeSpan.FromSeconds(int.Parse(match.Groups[1].Value));
			}
			catch (Exception ex)
			{
				 Logger.CreateLogger(nameof(PageConstructor)).Error(ex); 
			};
			return null;
		}

		private static bool StringComaperStart(string strMain, string str1, string str2) => strMain.StartsWith(str1, StringComparison.InvariantCultureIgnoreCase) || strMain.StartsWith(str2, StringComparison.InvariantCultureIgnoreCase);

		private static Hint GetHint(HtmlNode hint)
		{
			if (!hint.ChildNodes.Any()) return null;

			var regexTimer = new Regex("\"StartCounter\":(\\d+)");

			try
			{
				var match = regexTimer.Matches(hint.InnerHtml).First() as Match;
				var timeToEnd = TimeSpan.FromSeconds(int.Parse(match.Groups[1].Value));

				return new Hint(hint.FirstChild.InnerText, null, timeToEnd);
			}
			catch (Exception ex)
			{
				Logger.CreateLogger(nameof(PageConstructor)).Error(ex);
			};
			return null;
		}

		private static Hints Hints(HtmlNode documentNode)
		{
			var mainDiv = documentNode.SelectSingleNode("//h3[@class='color_bonus' or @class='color_correct']")?.ParentNode;
			if (mainDiv == null)
			{
				mainDiv = documentNode.SelectSingleNode("//div[@class='spacer']")?.ParentNode;
				if (mainDiv == null)
					return new Hints();
			}

			var hints = new Hints();

			HtmlNode temp = null;
			foreach (var hint in mainDiv.ChildNodes)
			{
				if (temp == null)
				{
					if (StringComaperStart(hint.InnerText, "Подсказка", "Penalty"))
					{
						switch (hint.Name)
						{
							case "h3":
								temp = hint;
								break;
							case "span":
							{
								var h = GetHint(hint);
								if (h != null)
									hints.Add(h);
								break;
							}
							default:
								temp = null;
								break;
						}
					}

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
							hints.Add(new Hint(temp.InnerHtml, hint.InnerText, TimeSpan.MinValue));
							temp = null;
							break;
						}
				}
			}

			if (temp != null)
			{
				var text = temp.InnerHtml;
				hints.Add(new Hint(text, text, TimeSpan.MinValue));
			}

			return hints;
		}

		private static Bonuses Bonuses(HtmlNode documentNode)
		{
			//var bonuses = new ();

			//<h3 class="color_bonus"> 
			var mainDiv = documentNode.SelectSingleNode("//h3[@class='color_bonus' or @class='color_correct']")?.ParentNode;

			var bonusesNodes = mainDiv?.ChildNodes.Where(x =>  (x.Attributes.Any(y => (y.Value == "color_bonus" || y.Value == "color_correct") && y.Name == "class") && x.Name == "h3"));
			return new Bonuses(bonusesNodes);
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

		private static string Task(HtmlNode documentNode, TypePage pageType)
		{
			var mainDiv = (pageType == TypePage.NotStarted)
				? documentNode.SelectSingleNode("//span[@id='Panel_TimerHolder']")
				: documentNode.SelectNodes("//h3")?.FirstOrDefault(x => x.InnerText == "Задание" || x.InnerText == "Task")?.NextSibling;

			if (mainDiv == null)
				return null;

			var sb = new StringBuilder();
			
			while ((mainDiv = mainDiv?.NextSibling) != null && mainDiv.Name != "div")
			{
				if (IsSystemNode(mainDiv))
					continue;

				//foreach (var node in mainDiv.ChildNodes.Where(x => !(IsSystemNode(x))))
				//sb.AppendLine(node.OuterHtml);

				sb.AppendLine(mainDiv.OuterHtml);
			}

			return WebHelper.RemoveSpaces(sb.ToString());
		}

		private static bool IsSystemNode(HtmlNode node) => node.Name == "script" || node.Name == "style";
	}
}
