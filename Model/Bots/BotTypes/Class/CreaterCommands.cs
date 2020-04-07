using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Model.Bots.BotTypes.Interfaces.Messages;

namespace Model.Bots.BotTypes.Class
{
	public class CreatorCommands
	{
		private const string NameGroup = "param";
		private const string SuffixTrue = "_on";
		private const string SuffixFalse = "_off";
		private const string SuffixDefault = "_";

		public string StartSplitterPattern { get; }

		public CreatorCommands(string[] startSplitter)
		{
			StartSplitterPattern = $"({startSplitter.Aggregate((x, y) => x + "|" + y)})";
		}

		public CreatorCommands(string startSplitter)
		{
			StartSplitterPattern = $"({startSplitter})";
		}

		public List<string> GetCommands(string str)
		{
			string @params = $"((\\s|^){StartSplitterPattern}(?'{NameGroup}'(\\w|\\d|_)+))*";
			var command = new List<string>();
			foreach (Match match in Regex.Matches(str, @params))
			{
				var groupParam = match.Groups.FirstOrDefault<Group>(x => x.Name == NameGroup)?.Captures; //.Select(x => x.Value.Replace("\"", string.Empty));
				if (groupParam == null) continue;

				command.AddRange(groupParam.Select(x => x.ToString()).Where(x => x != string.Empty).Select(x => x.ToString()));
			}
			return command;
		}

		public List<IMessageCommand> CreateCommands(string text, List<string> commands)
		{
			if (string.IsNullOrEmpty(text))
				return null;

			var result = new List<IMessageCommand>();
			foreach (var command in commands)
			{
				var paramsPattern = ($"{StartSplitterPattern}{command}([\\s-[\r\n]]+(?'{NameGroup}'([^\\s\n\r\"]+)|(\"[^\"]*\")))*");

				if (command.EndsWith(SuffixTrue))
				{
					result.Add(new MessageCommand(command.Substring(0, command.Length - SuffixTrue.Length).ToLower(), true.ToString()));
					continue;
				}
				
				if (command.EndsWith(SuffixFalse))
				{
					result.Add(new MessageCommand(command.Substring(0, command.Length - SuffixFalse.Length).ToLower(), false.ToString()));
					continue;
				}

				if (command.EndsWith(SuffixDefault))
				{
					var msgCommand = new MessageCommand(command.Substring(0, command.Length - SuffixDefault.Length).ToLower());
					result.Add(msgCommand);
					continue;
				}
				
				
				var comms = new List<string>();
				var name = command.ToLower();

				if (command.Contains("_"))
				{
					var array = command.Split('_');
					name = array[0].ToLower();
					comms = array.Skip(1).ToList();
				}

				foreach (Match match in Regex.Matches(text, paramsPattern))
				{
					var groupParam = match.Groups.FirstOrDefault<Group>(x => x.Name == NameGroup)?.Captures.Select(x => x.Value.Replace("\"", string.Empty)).ToArray();
					comms.AddRange(groupParam);
					result.Add(new MessageCommand(name, comms));
				}
			}
			return result;
		}
	}
}
