using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Nastya
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.OutputEncoding = Encoding.UTF8;
			StartBot();
		}
		private static void StartBot()
		{
			var startBot = new ManagerBots();
			startBot.Wait();
		}
	}
}