using System;
using Model.Types.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Model.Types.Class
{
	public class CreaterCommands
	{
		private const string NameGroup = "param";
		private const string suffixTrue = "_on";
		private const string suffixFalse = "_off";
		private const string suffixDefault = "_";

		public string StartSpliterPattern { get; }

		public CreaterCommands(string[] startSpliter)
		{
			StartSpliterPattern = $"({startSpliter.Aggregate((x, y) => x + "|" + y)})";
		}

		public CreaterCommands(string startSpliter)
		{
			StartSpliterPattern = $"({startSpliter})";
		}

		public List<string> GetCommands(string str)
		{
			Console.WriteLine(str);
			string @params = $"((\\s|^){StartSpliterPattern}(?'{NameGroup}'(\\w|\\d|_)+))*";
			var command = new List<string>();
			foreach (Match match in Regex.Matches(str, @params))
			{
				var groupParam = match.Groups.FirstOrDefault<Group>(x => x.Name == NameGroup)?.Captures; //.Select(x => x.Value.Replace("\"", string.Empty));
				if (groupParam != null)
					foreach (var x in groupParam)
					{
						if (x.ToString() != string.Empty)
							command.Add(x.ToString());
					}
			}
			return command;
		}

		public List<IMessageCommand> CreateCommands(string text, List<string> commands)
		{
			Console.WriteLine("-----------");
			if (string.IsNullOrEmpty(text))
				return null;

			Console.WriteLine(text, string.Join("\n", commands));

			var result = new List<IMessageCommand>();
			foreach (var command in commands)
			{
				string paramsPattern = ($"{StartSpliterPattern}{command}([\\s-[\r\n]]+(?'{NameGroup}'([^\\s\n\r\"]+)|(\"[^\"]*\")))*");

				if (command.EndsWith(suffixTrue))
				{
					result.Add(new MessageCommand(command.Substring(0, command.Length - suffixTrue.Length).ToLower(), true.ToString()));
				}
				else if (command.EndsWith(suffixFalse))
				{
					result.Add(new MessageCommand(command.Substring(0, command.Length - suffixFalse.Length).ToLower(), false.ToString()));
				}
				else if (command.EndsWith(suffixDefault))
				{
					var msgCommand = new MessageCommand(command.Substring(0, command.Length - suffixDefault.Length).ToLower());
					result.Add(msgCommand);
				}
				else
				{
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
			}
			return result;
		}

	}
}
