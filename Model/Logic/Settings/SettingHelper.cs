using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Model.BotTypes.Class;
using Model.BotTypes.Class.Ids;
using Model.BotTypes.Enums;
using Model.Files;
using Model.Files.FileTokens;
using Model.Logic.Model;
using Model.Logic.Settings.Classes;

namespace Model.Logic.Settings
{
	public class SettingHelper : ISettings, ISettingValues
	{
		private readonly object _lockobj = new object();
		private IChatFile _settingsFile;

		private string Path; // ToDo fromSetting
		public string GetValue(string name, string @default = default) => Settings.GetValue(name.ToLower(), @default);
		public bool GetValueBool(string name, bool @default = default) => Settings.GetValueBool(name.ToLower(), @default);
		public long GetValueLong(string name, long @default = default) => Settings.GetValueLong(name.ToLower(), @default);
		public Guid GetValueGuid(string name, Guid @default = default) => Settings.GetValueGuid(name.ToLower(), @default);
		public IMessageId GetIMessageId(string name, IMessageId @default = null) => Settings.GetMessageId(name.ToLower(), @default);

		public Settings Settings { get; }
		public IChatId ChatGuid  => new ChatGuid(Settings.ChatGuid);
		public TypeGame TypeGame => Settings.TypeGame;
		public ISettingsBraille Braille { get; }
		public ISettingsTest Test { get; }
		public ISettingsCoordinates Coordinates { get; }
		public ISettingsGame Game { get; }

		public IDlSettingsGame DlGame { get; }
		public IDzzzrSettingsGame DzzzrGame { get; }


		public ISettingsWeb Web { get; }
		public ISettingsPage Page { get; }
		public IChatFileFactory FileChatWorker { get; }
		
		public string NotExistFile(string ext) => NotExistFile(ext, ChatGuid);

		public static string NotExistFile(string ext, IChatId chatId) => FileHelper.GetNotExistFile(ext, chatId.ToString());

		public void SetValue(string name, string value)
		{
			Settings.SetValue(name, value);
			Save();
		}

		public SettingHelper(IChatId chatId, string directory)
		{
			FileChatWorker = new ChatFileFactory(chatId, directory); //This 
			_settingsFile = FileChatWorker.SettingsFile();
			Path = directory;

			// FileChatWorker = new LocalChatFileWorker(chatId);
			Braille = new BrailleSettings(this);
			Test = new TestSettings(this);
			Coordinates = new CoordinatesSettings(this);
			Game = new GameSettings(this);
			Web = new WebSettings(this);
			Page = new PageSettings(this);

			DlGame = new GameDlSettings(this);
			DzzzrGame = new GameDzzzrSettings(this);


			if (!_settingsFile.Exists())
			{
				Settings = new Settings(chatId.GetId);
				Save();
			}

			lock (_lockobj)
			{
				Settings = _settingsFile.Read<Settings>();
				Settings.SetDictionary();
			}
		}

		public TypeGame SetUri(string uri)
		{
			TypeGame result = TypeGame.Unknown;
			try
			{
				Settings.SetValue(Const.Game.Site, uri.ToString());
				result = UrlService.PrivateSetUri(this, uri);
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
			// var file = System.IO.Path.Combine(directory, chatId +".xml");

			Settings.Clear();
			Settings.SetList();

			lock (_lockobj)
			{
				_settingsFile.Delete();
			}
		}

		private void Save()
		{
			string chatId =  Settings.ChatGuid.ToString();

			var directory = System.IO.Path.Combine(Path, chatId.ToString());
			var file = System.IO.Path.Combine(directory, chatId + ".xml");
			Settings.SetList();
			
			_settingsFile.Save(Settings);
		}
	}
}
