namespace Model.Logic.Settings.Classes
{
	public class GameDlSettings : SettingValues2, IDlSettingsGame
	{
		// private bool sturm;

		public GameDlSettings(ISettingValues settingsValues) : base(settingsValues)
		{

		}

		public string TimeFormat {
			get => SettingsValues.GetValue(Const.DlGame.TimeFormat, "hh:mm:ss");
			set => SettingsValues.SetValue(Const.DlGame.TimeFormat, value);
		}

		public bool Sturm
		{
			get => SettingsValues.GetValueBool(Const.DlGame.Sturm);
			set => SettingsValues.SetValue(Const.DlGame.Sturm, value.ToString());
		}
	}
}