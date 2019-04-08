using Model.Logic.Settings;
using Model.Types.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Types.Class
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
