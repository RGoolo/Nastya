namespace Model.Logic.Settings.Classes
{
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
}