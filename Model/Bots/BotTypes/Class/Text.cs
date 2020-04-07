namespace Model.Bots.BotTypes.Class
{

	public class TexterSettings
	{
		public int MaxParsePicture { get; } = 10;

		public string LocalFiles { get; set; }


		public static TexterSettings Default = new TexterSettings();
	}

	public class Texter
	{
		public bool Html { get; private set; }
		public bool ReplaceCoordinates { get; }
		public bool ReplaceResources { get; } = true;
		public string Text { get; private set; }
		public TexterSettings Settings { get; set; } = TexterSettings.Default;
		public Texter Replace(string text, bool html)
		{
			Html = html;
			Text = text;

			return this;
		}
		
		public Texter (string text, bool html = false, bool replaceCoordinates = true)
		{
			Text = text;
			Html = html;
			ReplaceCoordinates = replaceCoordinates;
		}

		public override string ToString() => Text;

		public static explicit operator Texter(string param)
		{
			return new Texter(param, false, false);
		}
	}
}
