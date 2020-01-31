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

	[Flags]
	public enum FileTypeFlags
	{
		Undefined = 0,
		
		IsLocal = 1 << 1,
		IsChat = 1 << 2,
		
		IsCustomFile = 1 << 3,

		Resources = 1 << 4,
		Settings = 1 << 5,
	}

	[Flags]
	public enum FileType
	{
		Internet,
		Local,

		Chat,

		Resources,
		Settings,

		CustomFile,
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
		PrevieLvl,
		Sound,
		NewHint,
		NewSpoiler,
		SendSectors,
		SendAllSectors,
		GameStarted,
		GameStoped,
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
		Discord,
	}

	[Flags]
	public enum TypeUser
	{
		User = 0x1,
		Developer = 0x2,
		Admin = 0x4,
	}
}
