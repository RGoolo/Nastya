namespace Model.Settings.Classes
{
	public class GameDlSettings : SettingValues2, IDlSettingsGame
	{
		// private bool sturm;

		public GameDlSettings(ISettingValues settingsValues) : base(settingsValues)
		{

		}

		public bool Sturm
		{
			get => SettingsValues.GetValueBool(Const.DlGame.Sturm);
			set => SettingsValues.SetValue(Const.DlGame.Sturm, value.ToString());
		}
	}
}