using System.Collections.Generic;
using HtmlAgilityPack;
using Web.Base;
using System.Text.RegularExpressions;
using Web.DL;

namespace Web.Game.Model

{
	public class Page
	{
		/// <summary>
		/// Заголовок задания
		/// </summary>
		public string LevelTitle;

		/// <summary>
		/// Текст задания
		/// </summary>
		public string Task;

		/// <summary>
		/// Ссылки на картинки из текста задания
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

		static int i = 0;

		/// <summary>
		/// ID задания в движке
		/// </summary>
		public string LevelId;

		protected HtmlDocument htmlDocument;

		public SectorsCollection Sectors;

		public Page(string html)
		{
			html = html.Trim();

			var htmlDocument = new HtmlDocument();
			htmlDocument.LoadHtml(html);
			this.htmlDocument = htmlDocument;

			if (getContentBlock() == null)
				return;

			SetLevelTitle();
			SetTask();
			SetLinks();
			SetImages();
			SetLevelId();
			SetLevelNumber();
			SetIsCodeReceived();
			SetSectors();
		}

		protected void SetSectors()
		{
			Sectors = new SectorsCollection();

			var contentBlock = getContentBlock();
			var Headers = contentBlock.SelectNodes("h3");

			var regexSectorsTitle = new Regex(@"(На уровне|Level has) (\d+).+");
			var regexSectorsRemain = new Regex(@"(?s)^.+(осталось закрыть|left ot close) (\d+).+$");
			foreach (HtmlNode headerNode in Headers)
			{
				var text = headerNode.InnerText;
				var matches = regexSectorsTitle.Matches(text);
				if (matches.Count == 0)
				{
					continue;
				}

				Sectors.CountSectors = regexSectorsTitle.Replace(matches[0].Value, "$1");
				
				Sectors.SectorsRemain = regexSectorsRemain.Replace(text, "$1");
				

				var currentNode = headerNode;
				do
				{
					currentNode = currentNode.NextSibling;
				} while (currentNode == null || currentNode.GetAttributeValue("class", "") != "cols-wrapper");

				var sectorsNodes = currentNode.SelectNodes(".//p");

				foreach (HtmlNode sectorNode in sectorsNodes)
				{
					var sector = new Sector();
					text = sectorNode.InnerText;
					sector.Name = text.Substring(0, text.IndexOf(':'));
					sector.Answer = text.Substring(text.IndexOf(':') + 1);

					sector.Accepted = sectorNode.SelectSingleNode("child::span[@class='color_correct']") != null;
					Sectors.Sectors.Add(sector);
				}

				return;
			}
		}

		protected void SetIsCodeReceived()
		{
			IsReceived = null;


			var rows = htmlDocument.DocumentNode.SelectNodes("//ul[@class='history']//li");

			if (null == rows || rows.Count == 0)
			{
				return;
			}

			if ("color_correct" == rows[0].GetAttributeValue("class", ""))
			{
				IsReceived = true;

				return;
			}

			if ("incorrect" == rows[0].GetAttributeValue("id", ""))
			{
				IsReceived = false;

				return;
			}
		}

		protected void SetLevelId()
		{
			var levelInput =
				htmlDocument.DocumentNode.SelectSingleNode(
					"/html/body/div[@class='container']//input[@name='LevelId']");
			LevelId = levelInput?.GetAttributeValue("value", "");
		}

		protected void SetLevelNumber()
		{
			var LevelNumberInput =
				htmlDocument.DocumentNode.SelectSingleNode(
					"/html/body/div[@class='container']//input[@name='LevelNumber']");
			LevelNumber = LevelNumberInput?.GetAttributeValue("value", "");
		}

		/// <summary>
		/// Распарсить ссылки в тексте задания
		/// </summary>
		protected void SetLinks()
		{
			Links = new List<ILink>();

			var linkNodes = this.getContentBlock()?.SelectNodes("//p[1]//a");
			if (null == linkNodes)
			{
				return;
			}

			foreach (HtmlNode linkNode in linkNodes)
			{
				Links.Add(new Link(linkNode));
			}
		}

		/// <summary>
		/// Распарсить url картинок в тексте задания
		/// </summary>
		protected void SetImages()
		{
			ImageUrls = new List<string>();

			var imageNodes = this.getContentBlock()?.SelectNodes("//p[1]//img");
			if (null == imageNodes)
			{
				return;
			}

			foreach (HtmlNode imageNode in imageNodes)
			{
				var src = imageNode.GetAttributeValue("src", "");
				if (src != "")
				{
					ImageUrls.Add(src);
				}
			}

		}

		/// <summary>
		/// Распарсить заголовок задания
		/// </summary>
		protected void SetLevelTitle()
		{
			var titleBlock = getContentBlock()?.SelectSingleNode("h2[1]");
			this.LevelTitle = titleBlock?.InnerText;
		}

		/// <summary>
		/// Распарсить текст задания
		/// </summary>
		protected void SetTask()
		{
			var taskBlock = getContentBlock()?.SelectSingleNode("p[1]");
			var tmp = taskBlock.Clone();
			var brNodes = tmp.SelectNodes("child::br");
			foreach (var node in brNodes)
			{
				node.InnerHtml = "\n";
			}

			Task = tmp?.InnerText;
		}

		protected HtmlNode getContentBlock()
		{
			return this.htmlDocument.DocumentNode.SelectSingleNode(
				"/html/body/div[@class='container']/div[@class='content']");
		}
	}

	public class Sector
	{
		public string Name;
		public string Answer;
		public bool Accepted;
	}

	public class SectorsCollection
	{
		public List<Sector> Sectors;

		/// <summary>
		///  Сколько всего секторов
		/// </summary>
		public string CountSectors;

		/// <summary>
		/// Сколько секторов осталось закрыть
		/// </summary>
		public string SectorsRemain;

		public SectorsCollection()
		{
			Sectors = new List<Sector>();
		}
	}
}