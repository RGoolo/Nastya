using Model.Types.Class;
using System;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace Nastya
{
	class Program
	{
		static void Main(string[] args)
		{
			StartBot();	
		}
	
		private static void StartBot()
		{
			var StartBot = new ManagerBots();
			StartBot.Wait();
		}
	}
}