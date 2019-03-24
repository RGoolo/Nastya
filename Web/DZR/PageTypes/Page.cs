using HtmlAgilityPack;
using System;
using System.Text;
using Web.DZR.PageTypes;
using System.Linq;

namespace Web.DZR

{
	public enum PageType
	{
		NotFound,
		GameNotStart,
		GameGo,
		GameFinished,
		YouAreNotDeclared,
		Break,
		TaskNotScheduled,
		MainGameFinished,
	}

	public enum AnswerType : byte
	{
		none, correct, notcorrect, repited
	}


	public partial class Page
	{
		private const string YouAreNotDeclared = "Мы рады, что вы заглянули. В настоящее время вы не заявлены ни в одной из ближайших игр.";

		public PageType Type;
		public AnswerType AnswerType;

		public string SysMessage;
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
			SetAnswerType();

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
			if (YouAreNotDeclared == SysMessage)
				return PageType.YouAreNotDeclared;

			if (!string.IsNullOrEmpty(CommentBeforeSystemMsg))
			{
				if (CommentBeforeSystemMsg == "<!-- Error code: 15 -->")
					return PageType.TaskNotScheduled;
				if (CommentBeforeSystemMsg == "<!-- Error code: 27 -->")
					return PageType.Break;
			}
			
			//if (: byte copntain )

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
					TimeToEnd = new DateTime().AddSeconds(long.Parse(timeToEnd) );
				}
				catch
				{

				}
				return;
			}
		}

		private void SetAnswerType()
		{
			if (string.IsNullOrEmpty(SysMessage))
			{
				AnswerType = AnswerType.none;
				return;
			}

			if (SysMessage.StartsWith("Код принят"))
				AnswerType = AnswerType.correct;
			else if (SysMessage.StartsWith("Код не принят"))
				AnswerType = AnswerType.notcorrect;
			else if(SysMessage.StartsWith("Вы уже вводили этот код"))
				AnswerType = AnswerType.repited;
			else
				AnswerType = AnswerType.none;
		}

		private void SetSysMessage()
		{
			var sysMsg = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='sysmsg']");
			if (sysMsg == null)
			{
				var NotDeclared = htmlDocument.DocumentNode.SelectSingleNode($"//body/table/tr/td[text() = '{YouAreNotDeclared}']");
				if (NotDeclared != null)
					SysMessage = YouAreNotDeclared;

				return;
			}

			CommentBeforeSystemMsg = sysMsg.PreviousSibling?.PreviousSibling?.InnerHtml;
			SysMessage = sysMsg.InnerText.Trim();
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