using Model.Bots.BotTypes.Enums;

namespace Model.Bots.BotTypes
{
	public static class TyperGameExtension
	{
		public static bool IsDummy(this TypeGame type)
		{
			return (type & TypeGame.Dummy) != TypeGame.Unknown;
		}
	}
}
