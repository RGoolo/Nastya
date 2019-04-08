using System;

namespace Model.Logic.Settings
{
	public interface ISettingValues
	{
		void SetValue(string name, string value);
		string GetValue(string name, string @default = default(string));
		//T GetValue<T>(string name, T @default);
		bool GetValueBool(string name, bool @default = default(bool));
		long GetValueLong(string name, long @default = default(long));
		Guid GetValueGuid(string name, Guid @default = default(Guid));
	}

	public class SettingValues2
	{
		protected ISettingValues SettingsValues { get; }
		public SettingValues2(ISettingValues settingsValues)
		{
			SettingsValues = settingsValues;
		}
	}

	public class BrailleSettings : SettingValues2, ISettingsBraille
	{
		public BrailleSettings(ISettingValues settingsValues) : base(settingsValues) { }

		public string BrailleText
		{
			get => SettingsValues.GetValue(Const.Braille.braille);
			set => SettingsValues.SetValue(Const.Braille.braille, value);
		}

		public string Braille8
		{
			get => SettingsValues.GetValue(Const.Braille.braille8);
			set => SettingsValues.SetValue(Const.Braille.braille8, value);
		}
	}

	public class TestSettings : SettingValues2, ISettingsTest
	{
		public TestSettings(ISettingValues settingsValues) : base(settingsValues) { }

		public string IsTest
		{
			get => SettingsValues.GetValue(Const.Test.IsTest);
			set => SettingsValues.SetValue(Const.Test.IsTest, value);
		}

		public string TestUri
		{
			get => SettingsValues.GetValue(Const.Test.TestUri);
			set => SettingsValues.SetValue(Const.Test.TestUri, value);
		}
	}

	public class CoordinatesSettings : SettingValues2, ISettingsCoordinates
	{
		public CoordinatesSettings(ISettingValues settingsValues) : base(settingsValues) { }

		public bool Coord
		{
			get => SettingsValues.GetValueBool(Const.Coordinates.Coord);
			set => SettingsValues.SetValue(Const.Coordinates.Coord, value.ToString());
		}
		public string ShowYandex
		{
			get => SettingsValues.GetValue(Const.Coordinates.ShowYandex);
			set => SettingsValues.SetValue(Const.Coordinates.ShowYandex, value);
		}
	}

	public class GameSettings : SettingValues2, ISettingsGame
	{
		public GameSettings(ISettingValues settingsValues) : base(settingsValues) { }

		public string Start
		{
			get => SettingsValues.GetValue(Const.Game.Start);
			set => SettingsValues.SetValue(Const.Game.Start, value);
		}
		public string Stop
		{
			get => SettingsValues.GetValue(Const.Game.Stop);
			set => SettingsValues.SetValue(Const.Game.Stop, value);
		}
		public string Clear {
			get => SettingsValues.GetValue(Const.Game.Clear);
			set => SettingsValues.SetValue(Const.Game.Clear, value); }

		public bool Send
		{
			get => SettingsValues.GetValueBool(Const.Game.Send);
			set => SettingsValues.SetValue(Const.Game.Send, value.ToString());
		}
		public string LvlText
		{
			get => SettingsValues.GetValue(Const.Game.LvlText);
			set => SettingsValues.SetValue(Const.Game.LvlText, value);
		}
		public string LvlAllText
		{
			get => SettingsValues.GetValue(Const.Game.LvlAllText);
			set => SettingsValues.SetValue(Const.Game.LvlAllText, value);
		}

		public string Login
		{
			get => SettingsValues.GetValue(Const.Game.Login);
			set => SettingsValues.SetValue(Const.Game.Login, value);
		}
		public string Password
		{
			get => SettingsValues.GetValue(Const.Game.Password);
			set => SettingsValues.SetValue(Const.Game.Password, value);
		}
		public string Uri
		{
			get => SettingsValues.GetValue(Const.Game.Uri);
			set => SettingsValues.SetValue(Const.Game.Uri, value);
		}

		public string Coord
		{
			get => SettingsValues.GetValue(Const.Game.Coord);
			set => SettingsValues.SetValue(Const.Game.Coord, value);
		}

		public string LastCodes
		{
			get => SettingsValues.GetValue(Const.Game.LastCodes);
			set => SettingsValues.SetValue(Const.Game.LastCodes, value);
		}
		public string Codes
		{
			get => SettingsValues.GetValue(Const.Game.Codes);
			set => SettingsValues.SetValue(Const.Game.Codes, value);
		}

		public string AllBonus
		{
			get => SettingsValues.GetValue(Const.Game.AllBonus);
			set => SettingsValues.SetValue(Const.Game.AllBonus, value);
		}
		public string Bonus
		{
			get => SettingsValues.GetValue(Const.Game.Bonus);
			set => SettingsValues.SetValue(Const.Game.Bonus, value);
		}

		public bool GameIsStart
		{
			get => SettingsValues.GetValueBool(Const.Game.GameIsStart);
			set => SettingsValues.SetValue(Const.Game.GameIsStart, value.ToString());
		}

		public string Level
		{
			get => SettingsValues.GetValue(Const.Game.Level);
			set => SettingsValues.SetValue(Const.Game.Level, value.ToString());
		}
		public bool Sturm
		{
			get => SettingsValues.GetValueBool(Const.Game.Sturm);
			set => SettingsValues.SetValue(Const.Game.Sturm, value.ToString());
		}
		public string Prefix
		{
			get => SettingsValues.GetValue(Const.Game.Prefix);
			set => SettingsValues.SetValue(Const.Game.Prefix, value);
		}

		public bool IsSendImg
		{
			get => SettingsValues.GetValueBool(Const.Game.IsSendImg);
			set => SettingsValues.SetValue(Const.Game.IsSendImg, value.ToString());
		}
		public bool IsSendVoice
		{
			get => SettingsValues.GetValueBool(Const.Game.IsSendVoice);
			set => SettingsValues.SetValue(Const.Game.IsSendVoice, value.ToString());
		}

		public bool CheckOtherTask
		{
			get => SettingsValues.GetValueBool(Const.Game.CheckOtherTask);
			set => SettingsValues.SetValue(Const.Game.CheckOtherTask, value.ToString());
		}
		public Guid SectorsMsg
		{
			get => SettingsValues.GetValueGuid(Const.Game.SectorsMsg);
			set => SettingsValues.SetValue(Const.Game.SectorsMsg, value.ToString());
		}
		public Guid AllSectorsMsg
		{
			get => SettingsValues.GetValueGuid(Const.Game.AllSectorsMsg);
			set => SettingsValues.SetValue(Const.Game.AllSectorsMsg, value.ToString());
		}
	}

	public class WebSettings : SettingValues2, ISettingsWeb
	{
		public WebSettings(ISettingValues settingsValues) : base(settingsValues) { }

		public string Domen {
			get => SettingsValues.GetValue(Const.Web.Domen);
			set => SettingsValues.SetValue(Const.Web.Domen, value);
		}
		public string GameNumber
		{
			get => SettingsValues.GetValue(Const.Web.GameNumber);
			set => SettingsValues.SetValue(Const.Web.GameNumber, value);
		}
		public string BodyRequest
		{
			get => SettingsValues.GetValue(Const.Web.BodyRequest);
			set => SettingsValues.SetValue(Const.Web.BodyRequest, value);
		}

		public string PasswordAu
		{
			get => SettingsValues.GetValue(Const.Web.PasswordAu);
			set => SettingsValues.SetValue(Const.Web.PasswordAu, value);
		}
		public string LoginAu
		{
			get => SettingsValues.GetValue(Const.Web.LoginAu);
			set => SettingsValues.SetValue(Const.Web.LoginAu, value);
		}
	}

	public class PageSettings : SettingValues2, ISettingsPage
	{
		public PageSettings(ISettingValues settingsValues) : base(settingsValues) { }

		public string LastLvl
		{
			get => SettingsValues.GetValue(Const.Page.LastLvl);
			set => SettingsValues.SetValue(Const.Page.LastLvl, value);
		}

	}
}