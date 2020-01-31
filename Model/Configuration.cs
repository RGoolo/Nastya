using Newtonsoft.Json;

namespace Model
{
	[JsonObject("main")]
	public class Configuration
	{

		[JsonProperty("something")]
		public string Name { get; set; }
		
	}
}