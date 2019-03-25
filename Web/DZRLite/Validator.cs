using System;
using System.Linq;
using System.Collections.Generic;
using Model.Logic.Settings;
using Web.Base;
using System.Text.RegularExpressions;
using Web.Game.Model;
using Model.Types.Class;

namespace Web.DZRLite
{
	public class Validator : BaseValidator
	{
		private string lastPage;
		public Validator(ISettings setting) : base(setting)
		{

		}

		public override void AfterSendCode(string html, string code, Guid? idMsg)
		{
			switch (code)
			{
				case "0":
				{
					string data = lastPage;
					Regex regex = new Regex(@"\<img.+?src=['\""](?<imgsrc>.+?)['\""].+?\>", RegexOptions.ExplicitCapture);
					MatchCollection matches = regex.Matches(data);

					var imagesUrl = matches
							.Cast<Match>()
							.Select(m => m.Groups["imgsrc"].Value)						;

					//var msg = new List<CommandMessage>();
						//msg.AddRange(imagesUrl.Select(x => CommandMessage.GetPhototMsg("http://lite.dzzzr.ru/spb/go/" + x, new Texter(x)); //ToDo replace spb!!!!
					
					//ToDo
					//SndMsg(msg);
					break;
				}
				case "1":
				{
					var msg = new List<CommandMessage>();
					var end = lastPage.IndexOf("<br>");
					var st = lastPage.Remove(end).IndexOf("<br>");

					var mhtml = lastPage.Substring(st + 18, end - st - 18).Replace("<span style='color:red'>", "<b>").Replace("span>", "b>!").Split(",").Select(x => x.Trim()).ToList();
					string codes = "";
					for (int i = 0; i < mhtml.Count; i++)
					{
						codes += ((mhtml[i].StartsWith("<"))
							? ""
							: (mhtml[i]) + $"!({i + 1})" + " ,");
					}
						//ToDo
						//msg.Add(new MessageText(codes == "" ? "Все коды сняты" : codes, withHtml: true));
						SndMsg(msg);
					break;
				}
				case "2":
				{
					var msg = new List<CommandMessage>();
					var end = lastPage.LastIndexOf("<br>");
					var st = lastPage.Remove(end).LastIndexOf("<br>");

					var mhtml = lastPage.Substring(st + 18, end - st - 18).Replace("<span style='color:red'>", "<b>").Replace("</span>", "!</b>").Split(",").Select(x => x.Trim()).ToList();
					string codes = "";
					for (int i = 0; i < mhtml.Count; i++)
					{
						codes += ((mhtml[i].StartsWith("<"))
							         ? (mhtml[i].Replace("!", $"({i + 1})"))
							         : (mhtml[i]) + $"(!{i + 1})") + " ,";
					}
					//ToDo
					//msg.Add(new MessageText(codes, withHtml: true));
					SndMsg(msg);
					break;
				}
			}
		}

		public override void SetNewPage(string html)
		{
			var st = html.LastIndexOf("<!--prequel-->");
			var end = html.LastIndexOf("<!--prequelEnd-->");
			html = html.Substring(st + 14, end - st - 14);
			lastPage = html;


			/* foreach (HtmlNode imageNode in htmlDocument)
             {
                 var src = imageNode.GetAttributeValue("src", "");
             }*/




			/*  Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);


            Encoding utf8 = Encoding.GetEncoding("utf-8");
            Encoding win1251 = Encoding.GetEncoding("windows-1251");

            byte[] utf8Bytes = win1251.GetBytes(html);
            byte[] win1251Bytes = Encoding.Convert(win1251, utf8, utf8Bytes);
            html = win1251.GetString(win1251Bytes);
            */



			//lastPage = Regex.Replace(html, "<[^>]+>", string.Empty);




			//throw new NotImplementedException();
		}

		public override string LogInContext() => $@"action=auth&login={Settings.Game.Login}&password={Settings.Game.Password}";

		public override string LogInUrl() => GetUrl();

		public override string GetContextSetCode(string code) => lastPage;

		public override string GetUrl() => $@"http://{Settings.Web.Domen}/{Settings.Web.BodyRequest}/go/?pin={Settings.Web.GameNumber}";
	}
}
