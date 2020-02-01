using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Model;
using Model.Logic.Settings;

namespace Nastya
{
	class Program
	{
		static void Main(string[] args)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", true, true);

			var config = builder.Build();

			var appConfig = config.GetSection("main").Get<Configuration>();
			SettingsHelper.Directory = appConfig.SettingsPath;

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