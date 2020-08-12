using BotModel.Bots.BotTypes.Enums;
using BotModel.Bots.BotTypes.Interfaces.Ids;
using BotModel.Settings;
using Model.Logic.Coordinates;

namespace Model.Settings
{
	public interface IChatService: IChatService0
	{
        TypeGame SetUri(string uri);	
		TypeGame TypeGame { get;  }

		ISettingsBraille Braille { get; }
		ISettingsTest Test { get; }
		ISettingsCoordinates Coordinates { get; }
		ISettingsGame Game { get;  }
		
		IDlSettingsGame DlGame { get; }
		IDzzzrSettingsGame DzzzrGame { get;}

		ISettingsWeb Web { get; }
		ISettingsPage Page { get;}

		IPointsFactory PointsFactory { get; }


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
		// bool Coord { get; set; }
		bool AddPicture { get; set; }

		string GoogleCred{  get; set; }
		string City { get; set; }
		IGoogleCoordinates Google { get; }
		IYandexCoordinates Yandex { get; }
	}

	public interface IGoogleCoordinates : ICoordinates
	{

	}
	
	public interface IYandexCoordinates : ICoordinates
	{

	}

	public interface ICoordinates
	{
		 string Name { get; set; }
		 string PointNameMe { get; set; }
		 string PointName { get; set; } 
		 bool Show { get; set; }
	}

	public interface IDlSettingsGame
	{
		bool Sturm { get; set; }
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
		bool AllowConnect { get; set; }
        bool AllowCodeAudio { get; set; }

        IMessageId SectorsMsg { get; set; }
		IMessageId AllSectorsMsg { get; set; }
	}

	public interface ISettingsWeb
	{
		string Domen { get; set; }
		string GameNumber { get; set; }
		string BodyRequest { get; set; }
		string DefaultUri { get; set; }
    }

	public interface ISettingsPage
	{
		string LastLvl { get; set; }
	}
}