using Model.BotTypes.Enums;

namespace Model.BotTypes
{
	public static class TyperGameExtension
	{
		public static bool IsDummy(this TypeGame type)
		{
			return (type & TypeGame.Dummy) != TypeGame.Unknown;
		}
	}
}
