using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model.BotTypes.Class;
using Web.Base;

namespace Web.DZR
{
	public class Codes : Dictionary<int, Code>
	{
		public string Name;

		public Codes(string s) //, Dictionary<string, Dictionary<int, Answer>> answers)
		{
			var text = s.Split(":");
			if (text.Length < 2)
				return;

			Name = text[0].Trim();
			var i = 0;
			var codes = s.Substring(text[0].Length + 1).Split(",");

			//	var myAnswers = answers.ContainsKey(Name) ? answers[Name] : new Dictionary<int, Answer>() ;
			var myAnswers = new Dictionary<int, Answer>();

			foreach (var code in codes)
				Add(++i, NewCode(code, i, myAnswers));
		}

		private Code NewCode(string code, int count, Dictionary<int, Answer> ans) 
			=> new Code(WebHelper.RemoveAllTag(code).Trim(), code.Contains("span"), count, ans.ContainsKey(count) ? ans[count] : null);

		public string Text(bool onlyNotAccepted = false, string splitter = "	")
		{
			return Text(this.Values.Where(x => !onlyNotAccepted || !x.Accepted), splitter);
		}

		private string Text(IEnumerable<Code> codes, string splitter = "	", bool useAnswer = false)
		{
			var sb = new StringBuilder();
			sb.Append(Name + ":\n");

			foreach (var code in codes)
				sb.Append($"{code.ToString(true)}{splitter}");
			return sb.ToString();
		}

		public IEnumerable<Code> AcceptedCode => this.Values.Where(x => x.Accepted);

		public string DiffText(IEnumerable<Code> newAccepteds, string splitter = "	")
		{
			if (!newAccepteds.Any())
				return null;

			return Text(newAccepteds);
		}

		public IEnumerable<Code> Diff(Dictionary<int, Code> oldCodes) =>
			this.AcceptedCode.Where(x => oldCodes.ContainsKey(x.Count) && !oldCodes[x.Count].Accepted).ToList();
		
	}
}

