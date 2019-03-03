using HtmlAgilityPack;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Web.Base;

namespace Web.DZR
{
	public class Spoiler
	{
		public List<LinkStruct> Urls = new List<LinkStruct>();
		public string GetPostForCode(string code) => postCode.Replace(_code_, code);
		public bool IsComplited;
		public string Text;

		private string _code_ = "_code_szxjkilzxscuijkl";
		private readonly string _defaulUri;

		private string postCode;
		
		public Spoiler(HtmlNode node, string defaulUri)
		{
			_defaulUri = defaulUri;
			IsComplited = node.InnerHtml.Contains("<!--beginSpoilerText-->");

			if (!IsComplited)
				SetSendForm(node);
			else
				SetText(node);

		}
		void SetText(HtmlNode node)
		{
			foreach (Match match in Regex.Matches(node.InnerHtml, "<!--beginSpoilerText-->((\\s|\\S)*)<!--endSpoilerText-->"))
			{
				Text = WebHelper.RemoveTag( match.Groups[1].Value);
				return;
			}
			return;

			var levelNumberEnd = node.InnerHtml.IndexOf("");
			if (levelNumberEnd != -1)
			{
				var levelNumberBegin = node.InnerHtml.IndexOf("");
				var startNumber = levelNumberBegin + "<!--beginSpoilerText-->".Length;

				var allinfoText = WebHelper.RemoveImg(WebHelper.RemoteTagToTelegram(node.InnerHtml.Substring(startNumber, levelNumberEnd - startNumber)), false, _defaulUri);
				Text = allinfoText.Item1;
				Urls = allinfoText.Item2;
			}
		}

		private static List<string> inputForms = new List<string>
		{
			"div/form/input", "form/input", "div/p/form/input", "p/form/input",
		};

		void SetSendForm(HtmlNode node)
		{
			StringBuilder sb = new StringBuilder();

			var name = "name";
			var value = "value";
			var text = "text";
			bool first = false;

			HtmlNodeCollection selectedSp = null;
			foreach (var form in inputForms )
			{
				selectedSp = node.SelectNodes(form);
				if (selectedSp != null)
					break;
			}

			if (selectedSp == null)
			{
					postCode = "";
					return;
			}

			foreach (var nod in selectedSp)
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
}
