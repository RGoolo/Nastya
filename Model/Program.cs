using System;
using System.IO;
using Microsoft.Extensions.Configuration;
namespace Model
{
	class Program
	{
		public static void Main(string[] args)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", true, true);

			var config = builder.Build();

			var appConfig = config.GetSection("main").Get<Configuration>();

			Console.WriteLine(appConfig.Name);
		}
	}
}