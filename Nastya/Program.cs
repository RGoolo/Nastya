using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Model;
using Model.Bots.BotTypes.Interfaces;
using Model.Logger;
using Model.Logic.Settings;

namespace Nastya
{
	class Program
	{
		static void Main(string[] args)
		{
            try
            {
                new Services().Start().Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
		}
    }

    public class Services
    {
        private void Configure()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true);

            var config = builder.Build();
            var appConfig = config.GetSection("main").Get<Configuration>();

            SettingsHelper.Directory = appConfig.SettingsPath;
            Logger.FileLog = Path.Combine(appConfig.LogPath, DateTime.Now.ToString("HH.mm.ss") + ".txt");

            Console.OutputEncoding = Encoding.UTF8;
        }

        public Task Start()
        {
            Configure();

            var bots = BotsFactory.Bots();
            
            var manager = new ManagerBots(bots);
            return manager.StartTask();
        }

        public Task StarBots(List<IBot> bots)
        {
            Configure();

            var manager = new ManagerBots(bots);
            return manager.StartTask();
        }
    }
}