namespace Web.DL
{
	public class Bonus
	{
		public string Name;
		public string Text;
		public bool IsReady;

		public Bonus(string name, string text)
		{
			Name = name;
			Text = text;
			IsReady = !string.IsNullOrEmpty(text);
		}
	}
}