using System.Collections.Generic;
using Xunit;
using Model.BotTypes.Class;
using Model.TelegramBot;

namespace UnitTest
{
	public class RegexTest
	{

		
		[Theory]
		[MemberData(nameof(TypeUsers))]
		public void TexterTest(string input, string output)
		{
			//var 
			var texter = new Texter(input, true);
			(var str, var model) = TelegramBot.GetText(texter);
			Assert.Equal(str, output);
		}

		public static IEnumerable<object[]> TypeUsers()
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
	}
}
