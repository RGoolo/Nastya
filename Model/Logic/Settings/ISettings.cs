using Model.Types.Interfaces;
using System;

namespace Model.Logic.Settings
{
	public interface ISettings
	{
		string DontExistFile(string ext);

		void SetValue(string name, string value);

		TypeGame SetUri(string uri);
		Guid ChatGuid { get;  }
		TypeGame TypeGame { get;  }
		void Clear();
		string GetValue(string name, string @default = default(string));

		IChatFileWorker FileWorker { get; }
		ISettingsBraille Braille { get; }
		ISettingsTest Test { get; }
		ISettingsCoordinates Coordinates { get; }
		ISettingsGame Game { get;  }
		ISettingsWeb Web { get; }
		ISettingsPage Page { get;}
	}

	public interface ISettingsBraille
	{
		string BrailleText { get; set; }
		string Braille8 { get; set; }
	}

	public interface ISettingsTest
	{
		string IsTest { get; set; }
		string TestUri { get; set; }
	}

	public interface ISettingsCoordinates
	{
		bool Coord { get; set; }
		string ShowYandex { get; set; }
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
		string Uri { get; set; }

		string Coord { get; set; }

		string LastCodes { get; set; }
		string Codes { get; set; }

		string AllBonus { get; set; }
		string Bonus { get; set; }


		bool GameIsStart { get; set; }

		string Level { get; set; }
		bool Sturm { get; set; }
		string Prefix { get; set; }

		bool IsSendImg { get; set; }
		bool IsSendVoice { get; set; }

		bool CheckOtherTask { get; set; }
	}

	public interface ISettingsWeb
	{
		string Domen { get; set; }
		string GameNumber { get; set; }
		string BodyRequest { get; set; }

		string PasswordAu { get; set; }
		string LoginAu { get; set; }
	}

	public interface ISettingsPage
	{
		string LastLvl { get; set; }
	}
}