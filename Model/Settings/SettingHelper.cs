using System;
using BotModel.Bots.BotTypes.Class.Ids;
using BotModel.Bots.BotTypes.Enums;
using BotModel.Bots.BotTypes.Interfaces.Ids;
using BotModel.Exception;
using BotModel.Files;
using BotModel.Files.FileTokens;
using BotModel.Settings;
using Model.Logic.Coordinates;
using Model.Settings.Classes;

namespace Model.Settings
{
	public class SettingHelper : IChatService
	{
		private readonly object _lockObj = new object();
		private IChatFile _settingsFile;

		public string GetValue(string name, string @default = default) => Settings.GetValue(name.ToLower(), @default);
		public bool GetValueBool(string name, bool @default = default) => Settings.GetValueBool(name.ToLower(), @default);
		public long GetValueLong(string name, long @default = default) => Settings.GetValueLong(name.ToLower(), @default);
		public Guid GetValueGuid(string name, Guid @default = default) => Settings.GetValueGuid(name.ToLower(), @default);
		public IMessageId GetIMessageId(string name, IMessageId @default = null) => Settings.GetMessageId(name.ToLower(), @default);

		public BotModel.Settings.Settings Settings { get; private set; }
		public IChatId ChatId  => new ChatGuid(Settings.ChatGuid);
		public TypeGame TypeGame => Settings.TypeGame;
		public ISettingsBraille Braille { get; private set; }
		public ISettingsTest Test { get; private set; }
		public ISettingsCoordinates Coordinates { get; private set; }
		public ISettingsGame Game { get; private set; }

		public IDlSettingsGame DlGame { get; private set; }
		public IDzzzrSettingsGame DzzzrGame { get; private set; }


		public ISettingsWeb Web { get; private set; }
		public ISettingsPage Page { get; private set; }
		public IPointsFactory PointsFactory { get; private set; }
		public IChatFileFactory FileChatFactory { get; private set; }
		
		public string NotExistFile(string ext) => NotExistFile(ext, ChatId);

		public static string NotExistFile(string ext, IChatId chatId) => FileHelper.GetNotExistFile(ext, chatId.ToString());

		public void SetValue(string name, string value)
		{
			Settings.SetValue(name, value);
			Save();
		}

		public SettingHelper(SettingHelper0 helper0)
		{
			FileChatFactory = helper0.FileChatFactory; //This 
			_settingsFile = FileChatFactory.SystemFile(SystemChatFile.Settings);

            Settings = helper0.Settings;
			// FileChatWorker = new LocalChatFileWorker(chatId);
			Braille = new BrailleSettings(helper0);
			Test = new TestSettings(helper0);
			Coordinates = new CoordinatesSettings(helper0);
			Game = new GameSettings(helper0);
			Web = new WebSettings(helper0);
			Page = new PageSettings(helper0);

			DlGame = new GameDlSettings(helper0);
			DzzzrGame = new GameDzzzrSettings(helper0);

			PointsFactory = new PointsFactory(this);
		}

		public TypeGame SetUri(string uri)
		{
			var result = TypeGame.Unknown;
			try
			{
				Settings.SetValue(Const.Game.Site, uri);
				result = UrlService.SetUri(this, uri);
				Settings.SetValue(Const.Web.DefaultUri, UrlService.GetDefaultUrl(this, result));
			}
			catch (GameException)
			{
			
			}
			finally
			{
				Settings.TypeGame = result;
				Save();
			}

			return result;
		}

		
		public void Clear()
		{
			Settings.Clear();
			Settings.SetList();

			lock (_lockObj)
			{
				_settingsFile.Delete();
			}
		}

		private void Save()
		{
			Settings.SetList();
			
			lock (_lockObj)
			{
				_settingsFile.Save(Settings);
			}
		}
	}
}
