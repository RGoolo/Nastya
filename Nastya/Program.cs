using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Model;
using Model.Bots.BotTypes.Interfaces;
using Model.Bots.BotTypes.Interfaces.Messages;
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
        private const string Format = "dd-MM-yy hh-mm-ss";

        private void Configure()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true);

            var config = builder.Build();
            var appConfig = config.GetSection("main").Get<Configuration>();

            if (!string.IsNullOrEmpty(appConfig.SettingsPath))
                SettingsHelper.Directory = appConfig.SettingsPath;

            Console.WriteLine($"{nameof(Environment.SpecialFolder.ApplicationData)}: {Environment.SpecialFolder.ApplicationData}");
            Console.WriteLine($"{nameof(SettingsHelper.Directory)}: {SettingsHelper.Directory}");

            if (!string.IsNullOrEmpty(appConfig.LogPath))
                Logger.FileLog = Path.Combine(appConfig.LogPath, $"log_{DateTime.Now.ToString(Format)}.txt");

            Console.OutputEncoding = Encoding.UTF8;
        }

        //ToDo: move to test.proj, Delete tmp folder;
        private void ConfigureTest()
        {
            var tempFolder = Guid.NewGuid().ToString();
            SettingsHelper.Directory = Path.Combine(Path.GetTempPath(), tempFolder,  "Resources");
            Logger.FileLog = Path.Combine(Path.GetTempPath(), tempFolder, "Log", $"log_{DateTime.Now}.txt");

            Console.OutputEncoding = Encoding.UTF8;
        }

        public Task Start()
        {
            Configure();

            var bots = BotsFactory.Bots();
            
            var manager = new ManagerBots(bots);
            return manager.StartTask();
        }

        public Task StarBots(List<IBot<IBotMessage>> bots)
        {
            ConfigureTest();

            var manager = new ManagerBots(bots);
            return manager.StartTask();
        }
    }
}