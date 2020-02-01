using System;

namespace Model.BotTypes.Enums
{
	[Flags]
	public enum MessageType : long
	{
		Undefined = 0x0, 
		Text = 0x2,
		Coordinates = 0x4,
		Photo = 0x8,
		Voice = 0x10,
		Video = 0x20,
		Document = 0x40,
		SystemMessage = 0x80,
		Edit = 0x100,

		WithResource = Photo | Voice | Video | Document,
		All = Undefined | Text | Photo | Coordinates | Voice | Video | Document
	}
}
