using System.Collections.Generic;
using System.Linq;
using Model.BotTypes.Interfaces;
using Model.BotTypes.Interfaces.Messages;

namespace Model.BotTypes.Class
{
	public class MessageCommand : IMessageCommand
	{
		public string Name { get; }
		public string FirstValue => Values.First();
		public bool SetDefaultValue { get;  }
		public List<string> Values { get; }
		public MessageCommand(string name, List<string> values)
		{
			Name = name;
			Values = values ?? new List<string>();
		}

		public MessageCommand(string name, string value)
		{
			Name = name;
			Values = (new List<string>{value});
		}

		public MessageCommand(string name)
		{
			Name = name;
			SetDefaultValue = true;
			Values = new List<string>();
		}
	}


}
