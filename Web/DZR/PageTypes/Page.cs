using System.Collections.Generic;
using HtmlAgilityPack;
using Web.Base;
using System;
using System.Text;
using Web.DZR.PageTypes;
using System.Linq;

namespace Web.DZR

{
	public enum PageType : byte
	{
		NotFound,
		GameNotStart,
		GameGo,
		GameFinished,
		YouAreNotDeclared,
		Break,
		TaskNotScheduled,
	}

	public class Page
	{
		private const string YouAreNotDeclared = "Мы рады, что вы заглянули. В настоящее время вы не заявлены ни в одной из ближайших игр.";

		public PageType Type;


		public string SysMesssage;
		private string CommentBeforeSystemMsg;
		public Tasks Tasks;
		public DateTime? TimeToEnd;
		//public string TaskText;

		protected HtmlDocument htmlDocument;
	
		private readonly string _defaulUrl;

		public Page(string html, string DefaultUrl)
		{
			_defaulUrl = DefaultUrl;

			html = html.Trim();
			htmlDocument = new HtmlDocument {OptionDefaultStreamEncoding = Encoding.GetEncoding(1251)};
			htmlDocument.LoadHtml(html);

			SetSysMessage();
			SetTimerToEnd();

			Type = CheckType();
			
			if (Type != PageType.GameGo)
				return;
			
			SetTask();

			if (Tasks == null || !Tasks.Any())
				Type = PageType.NotFound;
		}

		private PageType CheckType()
		{
			if (YouAreNotDeclared == SysMesssage)
				return PageType.YouAreNotDeclared;

			if (!string.IsNullOrEmpty(CommentBeforeSystemMsg))
			{
				if (CommentBeforeSystemMsg == "<!-- Error code: 15 -->")
					return PageType.TaskNotScheduled;
				if (CommentBeforeSystemMsg == "<!-- Error code: 27 -->")
					return PageType.Break;
			}
			
			return PageType.GameGo;
		}

		private void SetTimerToEnd()
		{
			string setTimeout = "window.setTimeout('countDown(";
			//<td id="clock" width="200" height="120">08:20</td>
			//window.setTimeout('countDown(528)',1000);
			var nodeScripts = htmlDocument.DocumentNode.SelectNodes("//script");
			if (nodeScripts == null)
				return;

			//   <script>
		//window.setTimeout('countDown(694)',1000);
			//</script>

			foreach(var scrits in nodeScripts)
			{
				var timeStart = scrits.InnerText.IndexOf(setTimeout, StringComparison.InvariantCultureIgnoreCase);
				if (timeStart == -1)
					continue;

				timeStart += setTimeout.Length;
				var timeEnd = scrits.InnerText.IndexOf(")", timeStart);

				var timeToEnd = scrits.InnerText.Substring(timeStart, timeEnd - timeStart);
				try
				{
					TimeToEnd = new DateTime().AddSeconds(long.Parse(timeToEnd) * 1000);
				}
				catch
				{

				}
				return;
			}
		}

		private void SetSysMessage()
		{
			var sysMsg = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='sysmsg']");
			if (sysMsg == null)
			{
				var NotDeclared = htmlDocument.DocumentNode.SelectSingleNode($"//body/table/tr/td[text() = '{YouAreNotDeclared}']");
				if (NotDeclared != null)
					SysMesssage = YouAreNotDeclared;

				return;
			}

			CommentBeforeSystemMsg = sysMsg.PreviousSibling?.PreviousSibling?.InnerHtml;
			SysMesssage = sysMsg.InnerText;
		}

		protected void SetTask()
		{
			var nodes = htmlDocument.DocumentNode.SelectNodes("//div[(@class='title' or @class='zad' or @class='codeform') and (not(../@class) or ../@class!='zad')]");
			if (nodes == null) return;

			Tasks = new Tasks(nodes, _defaulUrl);
		}
	
		public enum TypeNode
		{
			task, title, codeform
		}
	}
}