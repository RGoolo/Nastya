using System;
using Model.BotTypes.Class;
using Model.BotTypes.Enums;
using Model.Files.FileTokens;

namespace Model.Logic.Settings
{
	public interface ISettings
	{
		string NotExistFile(string ext);

		void SetValue(string name, string value);
		string GetValue(string name, string @default = default(string));
		IChatId ChatGuid { get; }

		TypeGame SetUri(string uri);	
		TypeGame TypeGame { get;  }

		void Clear();
		
		IChatFileFactory FileChatWorker { get; }
		ISettingsBraille Braille { get; }
		ISettingsTest Test { get; }
		ISettingsCoordinates Coordinates { get; }
		ISettingsGame Game { get;  }
		
		IDlSettingsGame DlGame { get; }
		IDzzzrSettingsGame DzzzrGame { get;}

		ISettingsWeb Web { get; }
		ISettingsPage Page { get;}

		//List<Answer> Answers { get; }
	}

	public interface ISettingsBraille
	{
		string BrailleText { get; set; }
		string Braille8 { get; set; }
	}

	public interface ISettingsTest
	{
		string IsTest { get;  }
		string TestUri { get; }
	}

	public interface ISettingsCoordinates
	{
		bool Coord { get; }
		string ShowYandex { get; }
	}

	public interface IDlSettingsGame
	{
		string TimeFormat { get; }
		bool Sturm { get;}
	}

	public interface IDzzzrSettingsGame
	{
		string Prefix { get; }
		bool CheckOtherTask { get; }

		string PasswordAu { get; set; }
		string LoginAu { get; set; }
	}

	public interface ISettingsGame
	{
		string Start { get; set; }
		string Stop { get; set; }
		string Clear { get; set; }

		bool Send { get; set; }
		string LvlText { get; set; }
		string LvlAllText { get; set; }

		string Login { get; set; }
		string Password { get; set; }
		string Site { get; set; }

		string Coord { get; set; }

		string LastCodes { get; set; }
		string Codes { get; set; }

		string AllBonus { get; set; }
		string Bonus { get; set; }
		bool UpdateAllBonus { get; set; }
		bool UpdateBonus { get; set; }
		bool GameIsStart { get; set; }

		string Level { get; set; }

		bool IsSendImg { get; set; }
		bool IsSendVoice { get; set; }

		IMessageId SectorsMsg { get; set; }
		IMessageId AllSectorsMsg { get; set; }
	}

	public interface ISettingsWeb
	{
		string Domen { get; set; }
		string GameNumber { get; set; }
		string BodyRequest { get; set; }

	}

	public interface ISettingsPage
	{
		string LastLvl { get; set; }
	}
}