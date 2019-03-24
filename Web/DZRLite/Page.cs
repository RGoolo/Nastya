using System.Collections.Generic;
using HtmlAgilityPack;
using Web.Base;
using System;
using System.Text;
using System.Linq;
using static Web.Base.WebHelper;

namespace Web.DZRLite
{
	public class Page //: BasePage
	{
		//static int i = 0;
		/// <summary>
		/// ID задания в движке
		/// </summary>
		//public string LevelId;
		public string AnswerText;
		protected HtmlDocument htmlDocument;
		public List<Task> _tasks = new List<Task>();
		private readonly string _defaulUrl;

		public Page(string html, string DefaultUrl)
		{
			_defaulUrl = DefaultUrl;
			html = html.Trim();

			htmlDocument = new HtmlDocument();
			htmlDocument.LoadHtml(html);
			SetTask();
			SetAnswer();
			//WriteTasks();
		}

		private void SetAnswer()
		{
			try
			{
				AnswerText = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='sysmsg']")?.InnerText;
			}
			catch (Exception ex)
			{ Console.WriteLine("qwseas:" + ex.Message + ex.StackTrace); };
		}

		private void WriteTasks()
		{
			string text = "";
			text = "AnswerText:" + AnswerText + "\n";

			foreach (var task in _tasks)
			{
				text += "**********************************************\n";
				text += (task.TitleText) + ".\n";
				text += ("Alias:" + task.Alias) + ".\n";
				text += ("LvlNumber:" + task.LvlNumber) + ".\n";
				text += "--------------------\n";
				var textTask = RemoveImg(task.Text);
				text += textTask.Item1 + "\n";
				foreach (var img in textTask.Item2)
				{
					text += img.Name + ":" + img.Url + "\n";
				}

				text += "--------------------\n";
				//Console.WriteLine(task.);
				text += "Spoiler:" + task.Spoiler?.Text + ".\nIsSpoiler" + task.Spoiler?.IsComplited + ".\n";

				text += "Codes:\n";
				foreach (var codes in task.Codes)
				{
					text += codes.Name + ":";
					foreach (var code in codes.ListCode)
						text += $"({code.Name}):{code.Accepted};";
					text += "\n";
				}
				foreach (var hint in task._hints)
				{
					text += (hint.Name + hint.Text + "\n");
				}
			}
			var a = Console.OutputEncoding;
			Console.OutputEncoding = Encoding.GetEncoding(1251);
			Console.WriteLine(text);
			Console.OutputEncoding = a;
		}

		/// <summary>
		/// Распарсить заголовок задания
		/// </summary>
		protected void SetLevelTitle()
		{
			//  var titleBlock = getContentBlock()?.SelectSingleNode("h2[1]");
			// this.LevelTitle = titleBlock?.InnerText;
		}

		protected void SetTask()
		{
			var nodes = htmlDocument.DocumentNode.SelectNodes("//div[(@class='grayBox')]");
			if (nodes == null)
				return;

			List<HtmlNode> sendNodes = new List<HtmlNode>();
			foreach (var node in nodes)
			{
				// if (node.InnerText == "Последние три события игры команды")
				//     continue;

				if (node.GetAttributeValue("class", "0") == "codeform")
				{
					try
					{
						_tasks.Add(new Task(node, _defaulUrl));
					}
					catch
					{

					}


				}
			}
		}
	}

	public class Task
	{
		private string _code_ = "_code_";

		public string Alias;
		public string TitleText;
		public string Text;
		public string LvlNumber;

		public Spoiler Spoiler;
		public List<Codes> Codes = new List<Codes>();

		public int NumberHint => _hints.Count;
		public List<HintStruct> _hints = new List<HintStruct>();
		public List<LinkStruct> Urls = new List<LinkStruct>();

		private string _defaulUri;


		private string PostForCode;
		public string GetPostForCode(string code)
		{
			return PostForCode.Replace(_code_, code);
		}

		public Task(HtmlNode node, string defaulUri)
		{
			_defaulUri = defaulUri;


			SetTask(node);


		}

		private void SetTask(HtmlNode node)
		{

			//Spoiler = Spoiler.GetSpoiler(node);
			SetCodes(node);
			SetSendForm(node.SelectSingleNode("/form"));

			var levelTextEnd = node.InnerHtml.IndexOf("Коды сложности");
			if (levelTextEnd != -1)
			{
				var levelTextBegin = node.InnerHtml.IndexOf("<!--prequel-->");
				var startNumber = levelTextBegin + "<!--prequel-->".Length;

				Text = node.InnerHtml.Substring(startNumber, levelTextEnd - startNumber);
			}
		}

		private void SetCodes(HtmlNode node)
		{
			int end = node.InnerHtml.LastIndexOf("Коды сложности");
			int startForm = node.InnerHtml.IndexOf("</div>", end);
			var split = node.InnerHtml.Substring(end, startForm - end).Split("<br>");
			if (split.Length < 3)
				return;
			split.SkipLast(1).Skip(1).ToList().ForEach(x => Codes.Add(new Codes(x)));
		}

		private void SetHint(HtmlNode nodeTitle, HtmlNode node)
		{
			_hints.Add(new HintStruct(nodeTitle, node));
		}

		private void SetSendForm(HtmlNode node)
		{
			StringBuilder sb = new StringBuilder();

			var name = "name";
			var value = "value";
			var text = "text";
			bool first = false;
			foreach (var nod in node.SelectNodes("input"))
			{
				var nameAtt = nod.GetAttributeValue(name, string.Empty);
				if (nameAtt == string.Empty)
					continue;

				if (first)
					sb.Append("&");

				sb.Append(nod.GetAttributeValue("type", string.Empty) == text
					? $"{nameAtt}={_code_}"
					: $"{nameAtt}={nod.GetAttributeValue(value, string.Empty)}");
				first = true;
			}

			PostForCode = sb.ToString();
		}
	}

	public class Code
	{
		public string Name;
		public bool Accepted;
		public Code(string name, bool accepted)
		{
			Name = name;
			Accepted = accepted;
		}
		public override string ToString()
		{
			if (Accepted)
				return $"({Name})+;";
			else
				return $"<b>({Name})-</b>;";
		}
	}

	public class Codes
	{
		public string Name;
		public List<Code> ListCode = new List<Code>();

		public Codes(string s)
		{
			var a = s.Split(":");
			if (a.Length < 2)
				return;

			Name = a[0].Trim();
			var codes = s.Substring(a[0].Length + 1).Split(",");
			foreach (var code in codes)
			{
				ListCode.Add(new Code(RemoveTag(code).Trim(), code.Contains("span")));
			}
		}
	}


	public class HintStruct
	{
		public string Name;
		public string Text;
		public HintStruct(HtmlNode nodeTitle, HtmlNode node)
		{
			Name = nodeTitle.InnerText;

			var begin1 = node.InnerHtml.IndexOf("<!--");
			var end1 = node.InnerHtml.IndexOf("-->", begin1) + 3;
			var begin2 = node.InnerHtml.IndexOf("<!--", end1);

			Text = (node.InnerHtml.Substring(end1, begin2 - end1));
		}
	}

	public class Spoiler
	{
		private string _code_ = "_code_";

		private string postCode;
		public string GetPostForCode(string code) => postCode.Replace(_code_, code);


		public static Spoiler GetSpoiler(HtmlNode node)
		{
			if (!node.InnerHtml.Contains("<form method=\"post\">") && !node.InnerHtml.Contains("beginSpoilerText"))
				return null;

			return new Spoiler(node);
		}

		public bool IsComplited;
		public string Text;

		private Spoiler(HtmlNode node)
		{
			IsComplited = node.InnerHtml.Contains("beginSpoilerText");

			if (!IsComplited)
				SetSendForm(node);
			else
				SetText(node);

		}

		void SetText(HtmlNode node)
		{
			var levelNumberEnd = node.InnerHtml.IndexOf("<!--endSpoilerText-->");
			if (levelNumberEnd != -1)
			{
				var levelNumberBegin = node.InnerHtml.IndexOf("<!--beginSpoilerText-->");
				var startNumber = levelNumberBegin + "<!--beginSpoilerText-->".Length;

				Text = node.InnerHtml.Substring(startNumber, levelNumberEnd - startNumber);
			}
		}

		void SetSendForm(HtmlNode node)
		{
			StringBuilder sb = new StringBuilder();

			var name = "name";
			var value = "value";
			var text = "text";
			bool first = false;

			foreach (var nod in node.SelectNodes("div/form/input"))
			{
				//sb.Append(nod.InnerHtml);
				var nameAtt = nod.GetAttributeValue(name, string.Empty);
				if (nameAtt == string.Empty)
					continue;

				if (first)
					sb.Append("&");

				sb.Append(nod.GetAttributeValue("type", string.Empty) == text
					? $"{nameAtt}={_code_}"
					: $"{nameAtt}={nod.GetAttributeValue(value, string.Empty)}");
				first = true;
			}

			postCode = sb.ToString();
		}
	}

	/* public abstract class LinkStruct
     {
         public string Url;
         public string Name;

         public LinkStruct(string url, string name)
         {

         }
     }

     public struct ImgStruct
     {
         public string Url;
         public string Name;
     }
     public struct AHref
     {

     }*/

	public enum TypeNode
	{
		task, title, codeform
	}
}