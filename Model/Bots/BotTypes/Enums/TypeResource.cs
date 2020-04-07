namespace Model.Bots.BotTypes.Enums
{
	public enum TypeResource
	{
		None = 0,
		Photo = (int)MessageType.Photo,
		Voice = (int)MessageType.Voice,
		Video = (int)MessageType.Video,
		Document = (int)MessageType.Document,
	}

	public static class TypeResourceEx
	{
		public static TypeResource Convert(this MessageType type)
		{
			return (TypeResource)(int)((type & MessageType.WithResource));
		}
	}

}