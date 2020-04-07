using System;
using System.Collections.Generic;

namespace Model.Bots.TelegramBot.Entity
{
	public class ChatAdministrations
	{
		public DateTime LastUpdate { get; set; }
		public List<int> UserIds { get; set; }
	}
}
