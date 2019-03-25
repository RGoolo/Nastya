using System;

namespace Model.Types.Enums
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

	public enum FileType
	{
		Uri, Local
	}

	public enum SystemType : long
	{
		None,
		NeedResource,
		FindCoords,
		PhotoToCoord,
		TextToCoord,
		DzrSectors,
		NewLvl 
	}

	public enum Notification
	{
		None = 0,
		NewLevel,
		NewHint,
		NewSpoiler,
		SendSectors,
		
	}

	public enum TypeResource
	{
		None = 0,
		Photo = (int)MessageType.Photo,
		Voice = (int)MessageType.Voice,
		Video = (int)MessageType.Video,
		Document = (int)MessageType.Document,
	}
	
	public enum TypeBot
	{
		Dummy,
		Telegram,
		Skype,
	}

	[Flags]
	public enum TypeUser
	{
		User = 0x1,
		Developer = 0x2,
		Admin = 0x4,
	}
}
