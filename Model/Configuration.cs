using Newtonsoft.Json;

namespace Model
{
	[JsonObject("main")]
	public class Configuration
	{
		[JsonProperty("settingsPath")]
		public string SettingsPath { get; set; }

		[JsonProperty("logPath")]
		public string logPath { get; set; }
	}
}