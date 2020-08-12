using System;
using System.Collections.Generic;
using BotModel.Bots.BotTypes.Class.Ids;
using BotModel.Bots.BotTypes.Interfaces.Ids;
using BotModel.Bots.CmdBot;
using Xunit;

namespace UnitTest.LoadTest
{
	public  class BotLoadTest
	{
		private Dictionary<IBotId, CmdBot> _bots;

		[Fact]
		public void LoadTest()
		{
			var dt = DateTime.Now;
			_bots = new Dictionary<IBotId, CmdBot>();
			for (int i = 0; i < 100; i++)
			{
				var guid = new BotGuid(Guid.NewGuid());
				_bots.Add(guid, new CmdBot(guid));
			}
			FillBots();
			Assert.True((DateTime.Now - dt).TotalSeconds < 1);
		}

		private void FillBots()
		{
			foreach (var bot in _bots.Values)
			{
			
			}

			foreach (var bot in _bots.Values)
				bot.Message(bot.Id.ToString());

			Console.WriteLine("FillBotsEnd");
		}

	/*	private void Bot_OnMessage(object sender, Model.Args.MessageEventArgs e)
		{
			Debug.WriteLine(e.Message.Text);
			(new BotMaper(TypeBot.Dummy)).OnMessage(e.Message);
		}*/
	}
}
