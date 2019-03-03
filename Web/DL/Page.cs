using System.Collections.Generic;
using HtmlAgilityPack;
using Web.Base;
using System.Text.RegularExpressions;
using System.Linq;
using System;
using System.Text;

namespace Web.DL
{
	public class Page
	{
		/// <summary>
		/// Сколько времени до конца задания.
		/// </summary>
		public DateTime TimeToEnd;

		/// <summary>
		/// Заголовок задания.
		/// </summary>
		public string LevelTitle;
		/// <summary>
		/// Текст задания
		/// </summary>
		public string Task;
		/// <summary>
		/// Ссылки на картинки из текста задания.
		/// </summary>
		public List<string> ImageUrls;
		/// <summary>
		/// Прочие ссылки из текста задания
		/// </summary>
		public List<ILink> Links;
		/// <summary>
		/// Порядковый номер задания
		/// </summary>
		public string LevelNumber;

		/// <summary>
		/// Код принят
		/// false: Неверный код
		//  true:  Верный код
		/// null:  Код не вводился
		/// </summary>
		public bool? IsReceived;

		//public virtual void SetHTML(string html);

		public SectorsCollection Sectors;
		/// <summary>
		/// ID задания в движке
		/// </summary>
		public string LevelId = default(string);
		public List<Bonus> Bonuses = new List<Bonus>();
		public List<Hint> Hints = new List<Hint>();
		public List<Link> Levels = new List<Link>();
		protected HtmlDocument htmlDocument;

		public Page(string html)
		{
			html = html.Trim();

			var htmlDocument = new HtmlDocument();
			htmlDocument.LoadHtml(html);
			this.htmlDocument = htmlDocument;

			if (GetContentBlock() == null)
				return;

			SetLevelTitle();
			SetTimeToEnd();
			SetTask();
			SetLinks();
			SetImages();
			SetLevelId();
			SetLevelNumber();
			SetIsCodeReceived();
			SetSectors();
			SetHint();
			SetBonus();
			SetLevels();
		}

		private void SetLevels()
		{
			var levels = htmlDocument.DocumentNode.SelectNodes("//h3[@class='section level']/li/div/a[@class!='block']");

			if (levels == null)
				return;

			levels.ToList().ForEach(x => Levels.Add(new Link(x)));
		}

		private void SetTimeToEnd()
		{
			var firstTimer = htmlDocument.DocumentNode.SelectSingleNode("//h3[@class='timer']")?.ParentNode;
			var regexTimer = new Regex("\"StartCounter\":(\\d+)");

			try
			{
				var match = regexTimer.Matches(firstTimer.InnerText).First() as Match;
				TimeToEnd = new DateTime().AddSeconds(int.Parse(match.Groups[1].Value));
			}
			catch { };
		}

		private bool StringComaper(string trMain, string str1, string str2) => trMain == str1 || trMain == str2;

		private bool StringComaperStart(string strMain, string str1, string str2) => strMain.StartsWith(str1) || strMain.StartsWith(str2);

		protected void SetHint()
		{
			var mainDiv = htmlDocument.DocumentNode.SelectSingleNode("//h3[@class='color_bonus' or @class='color_correct']")?.ParentNode;
			if (mainDiv == null)
				return;

			//var readyhint = mainDiv.ChildNodes.Where(x => x.Name == "h3" && x.InnerText.StartsWith("Подсказка"));
			//var readyhint = mainDiv.ChildNodes.Where(x => x.Name == "h3" && x.InnerText.StartsWith("Подсказка"));

			HtmlNode temp = null;
			foreach (var hint in mainDiv.ChildNodes)
			{
				if (temp == null)
				{
					if (hint.Name == "h3" && StringComaperStart(hint.InnerText, "Подсказка", "Penalty"))
						temp = hint;
					else if (hint.Name == "span" && StringComaperStart(hint.InnerText, "Подсказка", "Penalty"))
					{
						if (hint.ChildNodes.Any())
						{
							var regexTimer = new Regex("\"StartCounter\":(\\d+)");

							try
							{
								var match = regexTimer.Matches(hint.InnerText).First() as Match;
								var timeToEnd = new DateTime().AddSeconds(int.Parse(match.Groups[1].Value));

								Hints.Add(new Hint(hint.FirstChild.InnerText, null, timeToEnd));
							}
							catch { };
						}
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
							continue;
						}
					case "p":
						{
							var text = WebHelper.RemoveTag(temp.InnerText).Replace("\r", "").Replace("\t", "");

							Hints.Add(new Hint(text, hint.InnerText, DateTime.MinValue));
							temp = null;
							break;
						}
				}
			}

			if (temp != null)
			{

				var text = WebHelper.RemoveTag(temp.InnerText).Replace("\r", "").Replace("\t", "");

				Hints.Add(new Hint(text, text, DateTime.MinValue));

			}

		}

		protected void SetBonus()
		{
			//<h3 class="color_bonus"> 
			var mainDiv = htmlDocument.DocumentNode.SelectSingleNode("//h3[@class='color_bonus' or @class='color_correct']")?.ParentNode;

			if (mainDiv == null)
				return;

			//HtmlNode temp = null;
			foreach (var bonus in mainDiv.ChildNodes.Where(x => x.Attributes.Any(y => (y.Value == "color_bonus" || y.Value == "color_correct") && y.Name == "class") || x.Name == "p"))
			{

				var text = WebHelper.ReplaceSpace(WebHelper.RemoveTag(bonus.InnerText), string.Empty);
				Bonuses.Add(text.StartsWith(" ") ? new Bonus(text.Substring(1), null) : new Bonus(text, null));
				continue;
				/*if (temp == null)
				{
					if (bonus.Name == "h3")
						temp = bonus;
					continue;
				}

				switch (bonus.Name)
				{
					case "h3":
					{
						var text = WebHelper.ReplaceSpace(WebHelper.RemoveTag(bonus.InnerText), string.Empty);
						Bonuses.Add(text.StartsWith(" ") ? new Bonus(text.Substring(1), null) : new Bonus(text, null));
						continue;
					}
					case "p":
					{
						var text = WebHelper.RemoveTag(bonus.InnerText).Replace("\r", "").Replace("\t", "");
						var startIndex = text.IndexOf("\n", text.IndexOf("\n", StringComparison.Ordinal) + 1, StringComparison.Ordinal);
						text = WebHelper.ReplaceSpace(text.Substring(0, startIndex), string.Empty);
						Bonuses.Add(new Bonus(text, (WebHelper.ReplaceSpace(WebHelper.RemoveTag(bonus.InnerText)))));
						break;
					}*/
			}
		}

		protected void SetSectors()
		{
			Sectors = new SectorsCollection();

			var contentBlock = this.GetContentBlock();
			var Headers = contentBlock.SelectNodes("h3");

			var regexSectorsTitle = new Regex(@"(На уровне|Level has) (\d+).+");
			var regexSectorsRemain = new Regex(@"(?s)^.+(осталось закрыть|left ot close) (\d+).+$");
			foreach (var headerNode in Headers)
			{
				var text = headerNode.InnerText;
				var matches = regexSectorsTitle.Matches(text);
				if (matches.Count == 0) continue;

				Sectors.CountSectors = regexSectorsTitle.Replace(matches[0].Value, "$2");

				Sectors.SectorsRemain = regexSectorsRemain.Replace(text, "$2");

				var currentNode = headerNode;
				do
				{
					currentNode = currentNode?.NextSibling;
				} while (currentNode == null || currentNode.GetAttributeValue("class", "") != "cols-wrapper");

				var sectorsNodes = currentNode.SelectNodes(".//p");

				foreach (var sectorNode in sectorsNodes)
				{
					var sector = new Sector();
					text = sectorNode.InnerText;
					sector.Name = text.Substring(0, text.IndexOf(':'));
					sector.Answer = text.Substring(text.IndexOf(':') + 1);

					sector.Accepted = sectorNode.SelectSingleNode("child::span[@class='color_correct']") != null;
					Sectors.Sectors.Add(sector);
				}
			}
		}

		protected void SetIsCodeReceived()
		{
			IsReceived = null;

			var rows = htmlDocument.DocumentNode.SelectNodes("//ul[@class='history']//li");

			if (null == rows || rows.Count == 0)
				return;

			if ("color_correct" == rows[0].GetAttributeValue("class", ""))
				IsReceived = true;
			else if ("incorrect" == rows[0].GetAttributeValue("id", ""))
				IsReceived = false;
			
		}

		protected void SetLevelId()
		{
			///html/body/div[@class='container']//input[@name='LevelId']
			var levelInput = htmlDocument.DocumentNode.SelectSingleNode("//input[@name='LevelId']");
			LevelId = levelInput?.GetAttributeValue("value", "");
		}

		protected void SetLevelNumber()
		{
			///html/body/div[@class='container']
			var levelNumberInput = htmlDocument.DocumentNode.SelectSingleNode("//input[@name='LevelNumber']");
			LevelNumber = levelNumberInput?.GetAttributeValue("value", "");
		}

		/// <summary>
		/// Распарсить ссылки в тексте задания
		/// </summary>
		protected void SetLinks()
		{
			Links = new List<ILink>();

			var linkNodes = this.GetContentBlock()?.SelectNodes("//p[1]//a");
			if (null == linkNodes)
				return;
			

			foreach (var linkNode in linkNodes)
			{
				if (!linkNode.OuterHtml.ToLower().Contains("\"/userdetails.aspx?"))
					Links.Add(new Link(linkNode));
			}
		}

		/// <summary>
		/// Распарсить url картинок в тексте задания
		/// </summary>
		protected void SetImages()
		{
			ImageUrls = new List<string>();

			var imageNodes = this.GetContentBlock()?.SelectNodes("//p[1]//img");
			if (null == imageNodes)
				return;

			foreach (var imageNode in imageNodes)
			{
				var src = imageNode.GetAttributeValue("src", "");
				if (src != "")
					ImageUrls.Add(src);
			}
		}

		/// <summary>
		/// Распарсить заголовок задания
		/// </summary>
		protected void SetLevelTitle()
		{
			var titleBlock = GetContentBlock()?.SelectSingleNode("h2[1]");
			LevelTitle = titleBlock?.InnerText;
		}

		/// <summary>
		/// Распарсить текст задания
		/// </summary>
		protected void SetTask()
		{
			var mainDiv = htmlDocument.DocumentNode.SelectNodes("//h3")?.FirstOrDefault(x => x.InnerText == "Задание" || x.InnerText == "Task");

			if (mainDiv?.NextSibling?.NextSibling == null)
				return;

			var sb = new StringBuilder();
			foreach (var node in mainDiv.NextSibling.NextSibling.ChildNodes.Where(x => x.Name != "script"))
				sb.Append(node.InnerHtml);

			Task = sb.ToString();
		}
		protected void SetTaskOld()
		{
			var taskBlock = GetContentBlock()?.SelectSingleNode("p[1]");
			if (taskBlock == null) return;
			
			var tmp = taskBlock.Clone();
			var brNodes = tmp.SelectNodes("child::br");
			if (brNodes == null)
				return;

			foreach (HtmlNode node in brNodes)
				node.InnerHtml = "\n";

			Task = tmp?.InnerText;
		}

		protected HtmlNode GetContentBlock() => htmlDocument.DocumentNode.SelectSingleNode("/html/body/div[@class='content']");
	}
}