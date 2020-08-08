using Model.Logic.Coordinates;

namespace Model.Logic.Settings.Classes
{

	public class GooogleSettings : SettingValues2, IGoogleCoordinates
	{
		public GooogleSettings(ISettingValues settingsValues) : base(settingsValues) { }

		public bool Show
		{
			get => SettingsValues.GetValueBool(Const.Coordinates.Google.Show, true);
			set => SettingsValues.SetValue(Const.Coordinates.Google.Show, value.ToString());
		}

		public string Name
		{
			get => SettingsValues.GetValue(Const.Coordinates.Google.NameLink, "[G]");
			set => SettingsValues.SetValue(Const.Coordinates.Google.NameLink, value.ToString());
		}
		public string PointName
		{
			get => SettingsValues.GetValue(Const.Coordinates.Google.NamePoints, "[G point]");
			set => SettingsValues.SetValue(Const.Coordinates.Google.NamePoints, value.ToString());
		}
		public string PointNameMe
		{
			get => SettingsValues.GetValue(Const.Coordinates.Google.NamePointsMe, "[G point me]");
			set => SettingsValues.SetValue(Const.Coordinates.Google.NamePointsMe, value.ToString());
		}
	}

	public class YandexSettings : SettingValues2, IYandexCoordinates
	{
		public YandexSettings(ISettingValues settingsValues) : base(settingsValues) { }

		public bool Show
		{
			get => SettingsValues.GetValueBool(Const.Coordinates.Yandex.Show, true);
			set => SettingsValues.SetValue(Const.Coordinates.Yandex.Show, value.ToString());
		}

		public string Name
		{
			get => SettingsValues.GetValue(Const.Coordinates.Yandex.NameLink, "[Y]");
			set => SettingsValues.SetValue(Const.Coordinates.Yandex.NameLink, value.ToString());
		}
		public string PointName
		{
			get => SettingsValues.GetValue(Const.Coordinates.Yandex.NamePoints, "[Y point]");
			set => SettingsValues.SetValue(Const.Coordinates.Yandex.NamePoints, value.ToString());
		}
		public string PointNameMe
		{
			get => SettingsValues.GetValue(Const.Coordinates.Yandex.NamePointsMe, "[Y point me]");
			set => SettingsValues.SetValue(Const.Coordinates.Yandex.NamePointsMe, value.ToString());
		}
	}

	public class CoordinatesSettings : SettingValues2, ISettingsCoordinates
	{
		public IYandexCoordinates Yandex { get; }

		public IGoogleCoordinates Google { get; }

		public CoordinatesSettings(ISettingValues settingsValues) : base(settingsValues)
		{
			Yandex = new YandexSettings(settingsValues);
			Google = new GooogleSettings(settingsValues);
		}

		public string City
		{
			get => SettingsValues.GetValue(Const.Coordinates.City);
			set => SettingsValues.SetValue(Const.Coordinates.City, value.ToString());
		}

		public bool AddPicture
		{
			get => SettingsValues.GetValueBool(Const.Coordinates.AddPicture, true);
			set => SettingsValues.SetValue(Const.Coordinates.AddPicture, value.ToString());
		}


		public string GoogleCred
		{
			get => SettingsValues.GetValue(Const.Coordinates.Google.GoogleCred);
			set => SettingsValues.SetValue(Const.Coordinates.Google.GoogleCred, value);
		}
	}
}