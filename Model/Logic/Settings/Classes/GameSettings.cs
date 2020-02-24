using Model.BotTypes.Class;

namespace Model.Logic.Settings.Classes
{




	public class GameSettings : SettingValues2, ISettingsGame
	{
		public GameSettings(ISettingValues settingsValues) : base(settingsValues) { }

		public string Start
		{
			get => SettingsValues.GetValue(Const.Game.Start);
			set => SettingsValues.SetValue(Const.Game.Start, value);
		}
		public string Stop
		{
			get => SettingsValues.GetValue(Const.Game.Stop);
			set => SettingsValues.SetValue(Const.Game.Stop, value);
		}
		public string Clear {
			get => SettingsValues.GetValue(Const.Game.Clear);
			set => SettingsValues.SetValue(Const.Game.Clear, value); }

		public bool Send
		{
			get => SettingsValues.GetValueBool(Const.Game.Send);
			set => SettingsValues.SetValue(Const.Game.Send, value.ToString());
		}
		public string LvlText
		{
			get => SettingsValues.GetValue(Const.Game.LvlText);
			set => SettingsValues.SetValue(Const.Game.LvlText, value);
		}
		public string LvlAllText
		{
			get => SettingsValues.GetValue(Const.Game.LvlAllText);
			set => SettingsValues.SetValue(Const.Game.LvlAllText, value);
		}

		public string Login
		{
			get => SettingsValues.GetValue(Const.Game.Login);
			set => SettingsValues.SetValue(Const.Game.Login, value);
		}
		public string Password
		{
			get => SettingsValues.GetValue(Const.Game.Password);
			set => SettingsValues.SetValue(Const.Game.Password, value);
		}
		public string Site
		{
			get => SettingsValues.GetValue(Const.Game.Site);
			set => SettingsValues.SetValue(Const.Game.Site, value);
		}

		public string Coord
		{
			get => SettingsValues.GetValue(Const.Game.Coord);
			set => SettingsValues.SetValue(Const.Game.Coord, value);
		}

		public string LastCodes
		{
			get => SettingsValues.GetValue(Const.Game.LastCodes);
			set => SettingsValues.SetValue(Const.Game.LastCodes, value);
		}
		public string Codes
		{
			get => SettingsValues.GetValue(Const.Game.Codes);
			set => SettingsValues.SetValue(Const.Game.Codes, value);
		}

		public string AllBonus
		{
			get => SettingsValues.GetValue(Const.Game.AllBonus);
			set => SettingsValues.SetValue(Const.Game.AllBonus, value);
		}
		public string Bonus
		{
			get => SettingsValues.GetValue(Const.Game.Bonus);
			set => SettingsValues.SetValue(Const.Game.Bonus, value);
		}

		public bool UpdateAllBonus
		{
			get => SettingsValues.GetValueBool(Const.Game.UpdateAllBonuses);
			set => SettingsValues.SetValue(Const.Game.UpdateAllBonuses, value.ToString());
		}
		public bool UpdateBonus
		{
			get => SettingsValues.GetValueBool(Const.Game.UpdateBonuses);
			set => SettingsValues.SetValue(Const.Game.UpdateBonuses, value.ToString());
		}

		public bool GameIsStart
		{
			get => SettingsValues.GetValueBool(Const.Game.GameIsStart);
			set => SettingsValues.SetValue(Const.Game.GameIsStart, value.ToString());
		}

		public string Level
		{
			get => SettingsValues.GetValue(Const.Game.Level);
			set => SettingsValues.SetValue(Const.Game.Level, value.ToString());
		}


		public bool IsSendImg
		{
			get => SettingsValues.GetValueBool(Const.Game.IsSendImg);
			set => SettingsValues.SetValue(Const.Game.IsSendImg, value.ToString());
		}
		public bool IsSendVoice
		{
			get => SettingsValues.GetValueBool(Const.Game.IsSendVoice);
			set => SettingsValues.SetValue(Const.Game.IsSendVoice, value.ToString());
		}

		public IMessageId SectorsMsg
		{
			get => SettingsValues.GetIMessageId(Const.Game.SectorsMsg);
			set => SettingsValues.SetValue(Const.Game.SectorsMsg, value.ToString());
		}
		public IMessageId AllSectorsMsg
		{
			get => SettingsValues.GetIMessageId(Const.Game.AllSectorsMsg);
			set => SettingsValues.SetValue(Const.Game.AllSectorsMsg, value.ToString());
		}
	}
}