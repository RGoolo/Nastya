using HtmlAgilityPack;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Web.Base;

namespace Web.DZR
{
	public class Spoiler
	{
		public string GetPostForCode(string code) => _postCode.Replace(_code_, code);
		public bool IsCompleted;
		public string Text;

		private string _code_ = "_code_szxjkilzxscuijkl";
		
		private string _postCode;
		
		public Spoiler(HtmlNode node)
		{
			IsCompleted = node.InnerHtml.Contains("<!--beginSpoilerText-->");

			if (!IsCompleted)
				SetSendForm(node);
			else
				SetText(node);

		}
		void SetText(HtmlNode node)
		{
			foreach (Match match in Regex.Matches(node.InnerHtml, "<!--beginSpoilerText-->((\\s|\\S)*)<!--endSpoilerText-->"))
			{
				Text = match.Groups[1].Value;
				return;
			}
		}

		private static List<string> inputForms = new List<string>
		{
			"div/form/input", "form/input", "div/p/form/input", "p/form/input",
		};

		void SetSendForm(HtmlNode node)
		{
			var sb = new StringBuilder();

			const string name = "name";
			const string value = "value";
			const string text = "text";
			var first = false;

			HtmlNodeCollection selectedSp = null;
			foreach (var form in inputForms )
			{
				selectedSp = node.SelectNodes(form);
				if (selectedSp != null)
					break;
			}

			if (selectedSp == null)
			{
					_postCode = "";
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

				//ToDo:String.Format()
				sb.Append(nod.GetAttributeValue("type", string.Empty) == text
					? $"{nameAtt}={_code_}"
					: $"{nameAtt}={nod.GetAttributeValue(value, string.Empty)}");
				first = true;
			}

			_postCode = sb.ToString();
		}
	}
}
