using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Web.Base;

namespace Web.DZR
{

	public class Task
	{
		private string _code_ = "_code_";

		public string Alias;
		public string TitleText;
		public string Text;
		public string LvlNumber;

		public Spoilers Spoilers { get; private set; }
		public List<Codes> Codes { get; } = new List<Codes>();

		public int NumberHint => _hints.Count;
		public List<Hint> _hints { get; } = new List<Hint>();
		public List<LinkStruct> Urls { get; } = new List<LinkStruct>();

		private readonly string _defaulUri;

		private string _postForCode;
		public string GetPostForCode(string code)
		{
			return _postForCode.Replace(_code_, code);
		}

		public Task(List<HtmlNode> nodes, string defaultUri)
		{
			_defaulUri = defaultUri;
			HtmlNode nodeTitle = null;
			bool wasTask = false;

			foreach (var node in nodes)
			{
				switch (node.GetAttributeValue("class", "0"))
				{
					case "codeform":
						SetSendForm(node);
						break;
					case "zad":
						if (!wasTask)
						{
							SetTask(nodeTitle, node);
							wasTask = true;
						}
						else
							SetHint(nodeTitle, node);
						break;
					case "title":
						nodeTitle = node;
						break;
					default:
						break;
				}
			}
		}


		private void SetTask(HtmlNode nodeTitle, HtmlNode node)
		{
			TitleText = WebHelper.RemoteTagToTelegram(nodeTitle.InnerHtml); //todo 
			var levelNumberEnd = nodeTitle.InnerHtml.IndexOf("<!--levelNumberEnd-->");
			if (levelNumberEnd != -1)
			{
				var levelNumberBegin = nodeTitle.InnerHtml.IndexOf("<!--levelNumberBegin-->");
				var startNumber = levelNumberBegin + "<!--levelNumberBegin-->".Length;

				LvlNumber = nodeTitle.InnerHtml.Substring(startNumber, levelNumberEnd - startNumber);
				Alias = nodeTitle.InnerHtml.Substring(0, levelNumberEnd).Replace("<!--levelNumberBegin-->", "");
			}

			Spoilers = new Spoilers(node, _defaulUri);

			SetCodes(node);

			var levelTextEnd = node.InnerHtml.IndexOf("<!--levelTextEnd-->");
			if (levelTextEnd != -1)
			{
				var levelTextBegin = node.InnerHtml.IndexOf("<!--levelTextBegin-->");
				var startNumber = levelTextBegin + "<!--levelTextBegin-->".Length;

				//var allinfoText = WebHelper.RemoveTag(WebHelper.RemoteTagToTelegram(node.InnerHtml.Substring(startNumber, levelTextEnd - startNumber)));
				var buffText = node.InnerHtml.Substring(startNumber, levelTextEnd - startNumber);//.Item1;
				buffText = WebHelper.RemoteTagToTelegram(buffText);

				var textTask = WebHelper.RemoveImg(buffText, false, _defaulUri);
				buffText = textTask.Item1;
				Urls.AddRange(textTask.Item2);
				
				foreach (var img in textTask.Item2.Where(x => x.TypeUrl == WebHelper.TypeUrl.Img))
				{
					buffText = buffText.Replace(img.Name, $"<a href=\"{img.Url}\">{img.Name}</a>");
					//Urls.AddRange(new ImgLinkStruct(img.Url, img.Name));
				}
				Text = buffText;

				//	Urls = allinfoText.Item2;
			}
			//if (Spoiler != null)
			//	Urls.AddRange(Spoiler.Urls);
		}

		private void SetCodes(HtmlNode node)
		{
			int end = node.InnerHtml.LastIndexOf("Коды сложности");
			var split = node.InnerHtml.Substring(end).Split("<br>");
			if (split.Length < 3)
				return;
			split.SkipLast(1).Skip(1).ToList().ForEach(x => Codes.Add(new Codes(x)));
		}

		private void SetHint(HtmlNode nodeTitle, HtmlNode node)
		{
			_hints.Add(new Hint(nodeTitle, node));
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

			_postForCode = sb.ToString();
		}
	}
}
/*
	
*/