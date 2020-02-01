﻿using Model.BotTypes.Enums;
using Model.BotTypes.Interfaces;
using Model.BotTypes.Interfaces.Messages;
using Model.Logic.Settings;

namespace Model.BotTypes.Class
{
	public class Answer
	{
		public TypeGame TypeGame { get; set; }
		public string Game { get; set; }
		public string Lvl { get; set; }
		public string Code { get; set; }
		public IUser User { get; set; }
		public string Sectors { get; set; }
		public int Number { get; set; }
	}
}
