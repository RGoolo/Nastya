namespace Model.Logic.Settings.Classes
{
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
}