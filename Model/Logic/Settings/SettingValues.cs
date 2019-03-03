namespace Model.Logic.Settings
{
	public interface ISettingValues
	{
		void SetValue(string name, string value);
		string GetValue(string name, string @default = default(string));
		//T GetValue<T>(string name, T @default);
		bool GetValueBool(string name, bool @default = default(bool));
		long GetValueLong(string name, long @default = default(long));
	}

	public class SettingValues2
	{
		protected ISettingValues SetingsValues { get; }
		public SettingValues2(ISettingValues setingsValues)
		{
			SetingsValues = setingsValues;
		}
	}

	public class BrailleSettings : SettingValues2, ISettingsBraille
	{
		public BrailleSettings(ISettingValues setingsValues) : base(setingsValues) { }

		public string BrailleText
		{
			get => SetingsValues.GetValue(Const.Braille.braille);
			set => SetingsValues.SetValue(Const.Braille.braille, value);
		}

		public string Braille8
		{
			get => SetingsValues.GetValue(Const.Braille.braille8);
			set => SetingsValues.SetValue(Const.Braille.braille8, value);
		}
	}

	public class TestSettings : SettingValues2, ISettingsTest
	{
		public TestSettings(ISettingValues setingsValues) : base(setingsValues) { }

		public string IsTest
		{
			get => SetingsValues.GetValue(Const.Test.IsTest);
			set => SetingsValues.SetValue(Const.Test.IsTest, value);
		}

		public string TestUri
		{
			get => SetingsValues.GetValue(Const.Test.TestUri);
			set => SetingsValues.SetValue(Const.Test.TestUri, value);
		}
	}

	public class CoordinatesSettings : SettingValues2, ISettingsCoordinates
	{
		public CoordinatesSettings(ISettingValues setingsValues) : base(setingsValues) { }

		public bool Coord
		{
			get => SetingsValues.GetValueBool(Const.Coordinates.Coord);
			set => SetingsValues.SetValue(Const.Coordinates.Coord, value.ToString());
		}
		public string ShowYandex
		{
			get => SetingsValues.GetValue(Const.Coordinates.ShowYandex);
			set => SetingsValues.SetValue(Const.Coordinates.ShowYandex, value);
		}
	}

	public class GameSettings : SettingValues2, ISettingsGame
	{
		public GameSettings(ISettingValues setingsValues) : base(setingsValues) { }

		public string Start
		{
			get => SetingsValues.GetValue(Const.Game.Start);
			set => SetingsValues.SetValue(Const.Game.Start, value);
		}
		public string Stop
		{
			get => SetingsValues.GetValue(Const.Game.Stop);
			set => SetingsValues.SetValue(Const.Game.Stop, value);
		}
		public string Clear {
			get => SetingsValues.GetValue(Const.Game.Clear);
			set => SetingsValues.SetValue(Const.Game.Clear, value); }

		public bool Send
		{
			get => SetingsValues.GetValueBool(Const.Game.Send);
			set => SetingsValues.SetValue(Const.Game.Send, value.ToString());
		}
		public string LvlText
		{
			get => SetingsValues.GetValue(Const.Game.LvlText);
			set => SetingsValues.SetValue(Const.Game.LvlText, value);
		}
		public string LvlAllText
		{
			get => SetingsValues.GetValue(Const.Game.LvlAllText);
			set => SetingsValues.SetValue(Const.Game.LvlAllText, value);
		}

		public string Login
		{
			get => SetingsValues.GetValue(Const.Game.Login);
			set => SetingsValues.SetValue(Const.Game.Login, value);
		}
		public string Password
		{
			get => SetingsValues.GetValue(Const.Game.Password);
			set => SetingsValues.SetValue(Const.Game.Password, value);
		}
		public string Uri
		{
			get => SetingsValues.GetValue(Const.Game.Uri);
			set => SetingsValues.SetValue(Const.Game.Uri, value);
		}

		public string Coord
		{
			get => SetingsValues.GetValue(Const.Game.Coord);
			set => SetingsValues.SetValue(Const.Game.Coord, value);
		}

		public string LastCodes
		{
			get => SetingsValues.GetValue(Const.Game.LastCodes);
			set => SetingsValues.SetValue(Const.Game.LastCodes, value);
		}
		public string Codes
		{
			get => SetingsValues.GetValue(Const.Game.Codes);
			set => SetingsValues.SetValue(Const.Game.Codes, value);
		}

		public string AllBonus
		{
			get => SetingsValues.GetValue(Const.Game.AllBonus);
			set => SetingsValues.SetValue(Const.Game.AllBonus, value);
		}
		public string Bonus
		{
			get => SetingsValues.GetValue(Const.Game.Bonus);
			set => SetingsValues.SetValue(Const.Game.Bonus, value);
		}

		public bool GameIsStart
		{
			get => SetingsValues.GetValueBool(Const.Game.GameIsStart);
			set => SetingsValues.SetValue(Const.Game.GameIsStart, value.ToString());
		}

		public string Level
		{
			get => SetingsValues.GetValue(Const.Game.Level);
			set => SetingsValues.SetValue(Const.Game.Level, value.ToString());
		}
		public bool Sturm
		{
			get => SetingsValues.GetValueBool(Const.Game.Sturm);
			set => SetingsValues.SetValue(Const.Game.Sturm, value.ToString());
		}
		public string Prefix
		{
			get => SetingsValues.GetValue(Const.Game.Prefix);
			set => SetingsValues.SetValue(Const.Game.Prefix, value);
		}

		public bool IsSendImg
		{
			get => SetingsValues.GetValueBool(Const.Game.IsSendImg);
			set => SetingsValues.SetValue(Const.Game.IsSendImg, value.ToString());
		}
		public bool IsSendVoice
		{
			get => SetingsValues.GetValueBool(Const.Game.IsSendVoice);
			set => SetingsValues.SetValue(Const.Game.IsSendVoice, value.ToString());
		}

		public bool CheckOtherTask
		{
			get => SetingsValues.GetValueBool(Const.Game.CheckOtherTask);
			set => SetingsValues.SetValue(Const.Game.CheckOtherTask, value.ToString());
		}
	}

	public class WebSettings : SettingValues2, ISettingsWeb
	{
		public WebSettings(ISettingValues setingsValues) : base(setingsValues) { }

		public string Domen {
			get => SetingsValues.GetValue(Const.Web.Domen);
			set => SetingsValues.SetValue(Const.Web.Domen, value);
		}
		public string GameNumber
		{
			get => SetingsValues.GetValue(Const.Web.GameNumber);
			set => SetingsValues.SetValue(Const.Web.GameNumber, value);
		}
		public string BodyRequest
		{
			get => SetingsValues.GetValue(Const.Web.BodyRequest);
			set => SetingsValues.SetValue(Const.Web.BodyRequest, value);
		}

		public string PasswordAu
		{
			get => SetingsValues.GetValue(Const.Web.PasswordAu);
			set => SetingsValues.SetValue(Const.Web.PasswordAu, value);
		}
		public string LoginAu
		{
			get => SetingsValues.GetValue(Const.Web.LoginAu);
			set => SetingsValues.SetValue(Const.Web.LoginAu, value);
		}
	}

	public class PageSettings : SettingValues2, ISettingsPage
	{
		public PageSettings(ISettingValues setingsValues) : base(setingsValues) { }

		public string LastLvl
		{
			get => SetingsValues.GetValue(Const.Page.LastLvl);
			set => SetingsValues.SetValue(Const.Page.LastLvl, value);
		}

	}
}