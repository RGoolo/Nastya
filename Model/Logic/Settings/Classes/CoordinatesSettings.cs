namespace Model.Logic.Settings.Classes
{
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

		public string GoogleCreads 
		{
			get => SettingsValues.GetValue(Const.Coordinates.GoogleCreads);
			set => SettingsValues.SetValue(Const.Coordinates.GoogleCreads, value);
		}
	}
}