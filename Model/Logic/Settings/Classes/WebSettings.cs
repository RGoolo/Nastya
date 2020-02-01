namespace Model.Logic.Settings.Classes
{
	public class WebSettings : SettingValues2, ISettingsWeb
	{
		public WebSettings(ISettingValues settingsValues) : base(settingsValues) { }

		public string Domen {
			get => SettingsValues.GetValue(Const.Web.Domen);
			set => SettingsValues.SetValue(Const.Web.Domen, value);
		}
		
		public string GameNumber
		{
			get => SettingsValues.GetValue(Const.Web.GameNumber);
			set => SettingsValues.SetValue(Const.Web.GameNumber, value);
		}
		
		public string BodyRequest
		{
			get => SettingsValues.GetValue(Const.Web.BodyRequest);
			set => SettingsValues.SetValue(Const.Web.BodyRequest, value);
		}
	}
}