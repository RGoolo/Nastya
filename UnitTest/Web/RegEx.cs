﻿using System.Collections.Generic;
using Xunit;
using Model.Types.Enums;
using Model.Types.Class;
using System.Text.RegularExpressions;
using Model.TelegramBot;

namespace UnitTest
{
	public class RegexTest
	{

		
		[Theory]
		[MemberData(nameof(TypeUsers))]
		public void CoordTest(string input, string output)
		{
			//var 
			var texter = new Texter(input, true);
			var str = TelegramBot.GetText(texter);
			Assert.Equal(str, output);

		}

		public static IEnumerable<object[]> TypeUsers()
		{
			yield return new object[] { "<b>bold</b>", "<b>bold</b>" };
			yield return new object[] { "<ba>bold</ba>", "bold" };

			yield return new object[] { "<ab>cd</ab>", "cd" };
			yield return new object[] { "<a href=\"link\">namelink</a>", "<a href=\"link\">namelink</a>" };
		

			yield return new object[] { "<i>italic</i>", "<i>italic</i>" };
			yield return new object[] { "<in>tI</in>", "tI" };

			yield return new object[] { "<h>col</h>", "col" };
			yield return new object[] { "<head>ahead</head>", "ahead" };
		}
	}
}
