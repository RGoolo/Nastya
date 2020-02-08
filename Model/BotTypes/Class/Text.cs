namespace Model.BotTypes.Class
{
	public class Texter
	{
		public bool Html { get; } 
		public string Text { get; }
		public int MaxParseLink { get; } = 10;

		public Texter (string text, bool html = false)
		{
			Text = text;
			Html = html;
		}

		public override string ToString() => Text;

		public static implicit operator Texter(string param)
		{
			return new Texter(param);
		}
	}
}
