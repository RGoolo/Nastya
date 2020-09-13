using BotModel.Settings.Classes;

namespace Model.Settings.Classes
{
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