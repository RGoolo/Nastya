using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using BotModel.Bots.BotTypes.Interfaces;
using BotModel.Bots.BotTypes.Interfaces.Messages;
using BotModel.Logger;
using BotModel.Settings;
using BotService;
using Microsoft.Extensions.Configuration;
using Model;
using Model.Settings;

namespace NightGameBot
{
	class Program
	{
		static void Main(string[] args)
        {
            new Random().Next();
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
                SettingsHelper0.Directory = appConfig.SettingsPath;

            Console.WriteLine($"{nameof(Environment.SpecialFolder.ApplicationData)}: {Environment.SpecialFolder.ApplicationData}");
            Console.WriteLine($"{nameof(SettingsHelper0.Directory)}: {SettingsHelper0.Directory}");

            if (!string.IsNullOrEmpty(appConfig.LogPath))
                Logger.FileLog = Path.Combine(appConfig.LogPath, $"log_{DateTime.Now.ToString(Format)}.txt");

            Console.OutputEncoding = Encoding.UTF8;
        }

        //ToDo: move to test.proj, Delete tmp folder;
        private void ConfigureTest()
        {
            var tempFolder = Guid.NewGuid().ToString();
            SettingsHelper0.Directory = Path.Combine(Path.GetTempPath(), tempFolder,  "Resources");
            Logger.FileLog = Path.Combine(Path.GetTempPath(), tempFolder, "Log", $"log_{DateTime.Now}.txt");

            Console.OutputEncoding = Encoding.UTF8;
        }

        public Task Start()
        {
            Configure();

            var bots = BotsFactory.Bots();
           
            var manager = new ManagerBots(bots, GeneratorTypes.Generate(), GeneratorInstance.GetInstances());
            return manager.StartTask();
        }

        public Task StarBots(List<IBot> bots)
        {
            ConfigureTest();

            var manager = new ManagerBots(bots, GeneratorTypes.Generate(), GeneratorInstance.GetInstances());
            return manager.StartTask();
        }
    }
}