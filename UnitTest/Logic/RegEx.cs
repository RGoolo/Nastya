using System;
using System.Collections.Generic;
using Xunit;
using Model.BotTypes.Class;
using Model.BotTypes.Class.Ids;
using Model.TelegramBot;

namespace UnitTest
{
	public class RegexTest
	{

		
		[Theory]
		[MemberData(nameof(Texts))]
		public void TexterTest(string input, string output)
		{
			//var 
			var texter = new Texter(input, true, false);
			var str = TelegramBot.GetNormalizeText(texter, new ChatGuid(Guid.Empty)).Text;
			Assert.Equal(str, output);
		}

		public static IEnumerable<object[]> Texts()
		{
			yield return new object[] { "<b>bold</b>", "<b>bold</b>" };
			yield return new object[] { "<b>bold</ba></ba>", "bold" };
			yield return new object[] { "<bd>bold</bd>", "bold" };

			yield return new object[] { "<ab>cd</ab>", "cd" };
			yield return new object[] { "<a href=\"link\">namelink</a>", "<a href=\"link\">namelink</a>" };
		

			yield return new object[] { "<i>italic</i>", "<i>italic</i>" };
			yield return new object[] { "<i><b>italic</b></i>", "<i><b>italic</b></i>" };
			yield return new object[] { "<i><b>italic</i></b>", "italic" };
			yield return new object[] { "<in>tI</in>", "tI" };

			yield return new object[] { "<h>col</h>", "col" };
			yield return new object[] { "<head>ahead</head>", "ahead" };

			yield return new object[] { "<head>abc", "abc" };
		}

		[Theory]
		[MemberData(nameof(Urls))]
		public void UrlTest(string url, string startUrl, string result)
		{
			var u = TelegramHtml.GetFullUrl(url, startUrl);
			Assert.Equal(u, result);
		}

		public static IEnumerable<object[]> Urls()
		{
			yield return new object[] { "https://1.png", "http://1.png", "https://1.png" };
			yield return new object[] { "http://1.png", "http://1.png", "http://1.png" };
			
			yield return new object[] { @"./1.html", @"D:\sites", @"D:\sites/1.html" };
			yield return new object[] { @"1.html", @"D:\sites", @"D:\sites/1.html" };
			yield return new object[] { @"../../1.html", @"D:\sites/a/b", @"D:\sites/1.html" };

			yield return new object[] { @"./1.html", @"https://sites", @"https://sites/1.html" };
			yield return new object[] { @"1.html", @"https://sites", @"https://sites/1.html" };
			yield return new object[] { @"../../1.html", @"https://sites/a/b", @"https://sites/1.html" };
		}
		
		[Theory]
		[MemberData(nameof(Imgs))]
		public void RemoveImgTest(string text, string linkName, string linkUrl)
		{
			/*var l = TelegramHtml.ImgToHref(text);
			Assert.Single(l.Item2);
			Assert.Equal(l.Item2[0].Name, linkName);
			Assert.Equal(l.Item2[0].Url, linkUrl);*/
		}

		public static IEnumerable<object[]> Imgs()
		{
			yield return new object[] { "<img src=\"images/news/news.gif\" border=\"0\">", "news.gif", "images/news/news.gif" };
			yield return new object[] { "<img src=\"gif\" border=\"0\">", "gif", "gif" };
			
		}
	}
}
