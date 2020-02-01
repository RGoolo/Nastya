namespace Model.BotTypes.Enums
{
	public enum TypeResource
	{
		None = 0,
		Photo = (int)MessageType.Photo,
		Voice = (int)MessageType.Voice,
		Video = (int)MessageType.Video,
		Document = (int)MessageType.Document,
	}
}