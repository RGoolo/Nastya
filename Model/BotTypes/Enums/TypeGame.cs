using System;

namespace Model.BotTypes.Enums
{
	[Flags, Serializable]
	public enum TypeGame
	{
		Unknown = 0x0,
		Dzzzr = 0x1,
		DeadLine = 0x2,
		Dummy = 0x4,
		Prequel = 0x8,
		Lite = 0x10,

		DzzzrLite = Lite | Dzzzr,
		DzzzrPrequel = Dzzzr | Prequel,
		DzzzrLitePrequel = Dzzzr | Prequel | Lite,
	}
}