namespace Model.Logic.Settings.Classes
{
	public class GameDzzzrSettings : SettingValues2, IDzzzrSettingsGame
	{
		public GameDzzzrSettings(ISettingValues settingsValues) : base(settingsValues) { }

		public string Prefix
		{
			get => SettingsValues.GetValue(Const.DzrGame.Prefix);
			set => SettingsValues.SetValue(Const.DzrGame.Prefix, value);
		}

		public bool CheckOtherTask
		{
			get => SettingsValues.GetValueBool(Const.DzrGame.CheckOtherTask);
			set => SettingsValues.SetValue(Const.DzrGame.CheckOtherTask, value.ToString());
		}

		public string PasswordAu
		{
			get => SettingsValues.GetValue(Const.DzrGame.PasswordAu);
			set => SettingsValues.SetValue(Const.DzrGame.PasswordAu, value);
		}
		public string LoginAu
		{
			get => SettingsValues.GetValue(Const.DzrGame.LoginAu);
			set => SettingsValues.SetValue(Const.DzrGame.LoginAu, value);
		}

	}
}