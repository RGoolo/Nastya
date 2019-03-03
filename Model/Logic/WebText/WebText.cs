namespace Model.Logic.WebText
{
	public class WebText
	{
		public string WitOutTags { get; set; }
		string TextForTelegram { get; set; }
		public string WithHtmlTags { get; set; }

		public WebText(string text)
		{

		}

		public string GetHtmlForTelegramm(string uri)
		{
			return WitOutTags;
		}

		public override string ToString()
		{
			return WitOutTags;
		}
	}
}
