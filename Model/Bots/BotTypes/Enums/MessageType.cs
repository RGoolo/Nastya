using System;

namespace Model.Bots.BotTypes.Enums
{
	[Flags]
	public enum MessageType : long
	{
		Undefined = 0x0, 
		Text = 1 << 2,
		Coordinates = 1 << 3,
		
		Photo = 1 << 4,
		Voice = 1 << 5,
		Video = 1 << 6,
		Document = 1 << 7,
		
		SystemMessage = 1 << 8,
		Edit = 1 << 9,

		WithResource = Photo | Voice | Video | Document,
		All = ~0,
    }
}
