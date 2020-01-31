using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Model.BotTypes.Class;
using Model.BotTypes.Class.Ids;
using Model.Files;
using Model.Files.FileTokens;
using Model.Logic.Model;

namespace Model.Logic.Settings
{
	public class SettingHelper : ISettings, ISettingValues
	{
		private readonly object _lockobj = new object();
		private static string Path => @"botseting"; // ToDo fromSetting
		public string GetValue(string name, string @default = default) => Settings.GetValue(name.ToLower(), @default);
		public bool GetValueBool(string name, bool @default = default) => Settings.GetValueBool(name.ToLower(), @default);
		public long GetValueLong(string name, long @default = default) => Settings.GetValueLong(name.ToLower(), @default);
		public Guid GetValueGuid(string name, Guid @default = default) => Settings.GetValueGuid(name.ToLower(), @default);
		public IMessageId GetIMessageId(string name, IMessageId @default = null)
		{
			return Settings.GetMessageId(name.ToLower(), @default);
		}

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

		public SettingHelper(IChatId chatId)
		{
			var file = System.IO.Path.Combine(Path, chatId.ToString(), chatId + ".xml");

			if (!System.IO.File.Exists(file))
			{
				Settings = new Settings(chatId.GetId);
				Save();
			}

			lock (_lockobj)
			{
				XmlSerializer formatter = new XmlSerializer(typeof(Settings));
				using (FileStream fs = new FileStream(file, FileMode.OpenOrCreate))
					Settings = (Settings)formatter.Deserialize(fs);

				Settings.SetDictionary();
			}

			// FileChatWorker = new LocalChatFileWorker(chatId);
			Braille = new BrailleSettings(this);
			Test = new TestSettings(this);
			Coordinates = new CoordinatesSettings(this);
			Game = new GameSettings(this);
			Web = new WebSettings(this);
			Page = new PageSettings(this);
			
			DlGame = new GameDlSettings(this);
			DzzzrGame = new GameDzzzrSettings(this);

			FileChatWorker = new ChatFileTokenFactory(chatId, "settings"); //This 
		}

		private bool StartWith(StringBuilder sb, string str, bool replace = false)
		{
			if (string.IsNullOrEmpty(str))
				return true;
			if (sb.Length < str.Length)
				return false;
			if (sb.ToString().StartsWith(str, StringComparison.CurrentCultureIgnoreCase))
			{
				if (replace)
					sb.Remove(0, str.Length);
				return true;

			}
			return false;
		}

		public TypeGame SetUri(string uri)
		{
			TypeGame result = TypeGame.Unknown;
			try
			{
				Settings.SetValue(Const.Game.Site, uri.ToString());
				result = PrivateSetUri(uri);

			}
			catch (GameException)
			{
				Settings.TypeGame = result;
				Save();
				throw;
			}
			catch
			{

			}

			Settings.TypeGame = result;
			Save();
			return result;
		}

		private TypeGame PrivateSetUri(string uri)
		{
			var dummy = "dummy:";
			var lite = "lite:";
			var dzzzr = "dzzzr:";
			var deadLine = "dl:";
			var prequel = "pr:";

			var result = TypeGame.Unknown;
			if (string.IsNullOrEmpty(uri))
				return result;

			StringBuilder newUri = new StringBuilder(uri);
			Settings.SetValue(Const.Web.Domen, newUri.ToString());

			if (StartWith(newUri, dummy, true))
			{
				result |= TypeGame.Dummy;
				if (StartWith(newUri, prequel, true))
					result |= TypeGame.Prequel;

				if (StartWith(newUri, lite, true))
					result |= TypeGame.Lite;
				else if (StartWith(newUri, dzzzr, true))
					result |= TypeGame.Dzzzr;
				else if (StartWith(newUri, deadLine, true))
					result |= TypeGame.DeadLine;

				Settings.SetValue(Const.Web.Domen, newUri.ToString());
				//Settings.SetValue(Const.Game.Uri, newUri.ToString());
				return result;
			}

			if (StartWith(newUri, "https://", true))
			{
				//empty
			}

			if (StartWith(newUri, "http://", true))
			{
				//empty
			}

			if (StartWith(newUri, "lite.dzzzr.ru", false))
			{
				result |= TypeGame.Lite;

				var site = newUri.ToString().Split('/');
				Settings.SetValue(Const.Web.Domen, site[0]);
				Settings.SetValue(Const.Web.BodyRequest, site[1]);

				if (uri.Contains("?pin="))
				{
					Settings.SetValue(Const.Web.GameNumber, int.Parse(site[3].Replace("?pin=", "")).ToString());
				}
				else
				{
					result |= TypeGame.Prequel;
					Settings.SetValue(Const.Web.GameNumber, "0");
				}

				return result;
			}

			if (StartWith(newUri, "classic.dzzzr.ru", false))
			{
				//classic.dzzzr.ru/spb/go/
				result |= TypeGame.Dzzzr;
				var site = newUri.ToString().Split('/');
				Settings.SetValue(Const.Web.BodyRequest, site[1]);
				Settings.SetValue(Const.Web.Domen, site[0]);

				if (uri.Contains("section=anons"))
					result |= TypeGame.Prequel;

				return result;
			}

			//demo.en.cx/GameDetails.aspx?gid=26569
			result |= TypeGame.DeadLine;
			if (uri.Contains("GameDetails"))
			{
				Settings.SetValue(Const.Web.GameNumber, newUri.ToString().Split("=")[1]);
				Settings.SetValue(Const.Web.Domen, newUri.ToString().Split("/")[0]);
				Settings.SetValue(Const.Web.BodyRequest, "gameengines/encounter/play");
				return result;
			}
			
			//demo.en.cx/gameengines/encounter/play/26569
			var correct = newUri.ToString().Split("/", StringSplitOptions.RemoveEmptyEntries);

			if (correct.Length < 4)
				throw new GameException("Не удалось номер распарить ссылку");

			if (!int.TryParse(correct.Last(), out int numberGame))
				throw new GameException("Не удалось номер игры получить");

			Settings.SetValue(Const.Web.GameNumber, numberGame.ToString());
			Settings.SetValue(Const.Web.Domen, correct[0]);
			var bodyRequest = correct.Skip(1).SkipLast(1).Aggregate((x, y) => x + "/" + y);
			//for (var i = 1; i < correct.Length - 1; i++)
			//bodyRequest += correct[i] + "/";
			Settings.SetValue(Const.Web.BodyRequest, bodyRequest);
			
			return result;
		}

		public void Clear()
		{
			var chatId = Settings.ChatGuid.ToString();
			var directory = System.IO.Path.Combine(Path,chatId);
			var file = System.IO.Path.Combine(directory, chatId +".xml");

			Settings.Clear();

			Settings.SetList();
			lock (_lockobj)
			{
				if (System.IO.File.Exists(file))
					System.IO.File.Delete(file);
			}
		}

		private void Save()
		{
			string chatId =  Settings.ChatGuid.ToString();

			var directory = System.IO.Path.Combine(Path, chatId.ToString());
			var file = System.IO.Path.Combine(directory, chatId + ".xml");

			Settings.SetList();
			lock (_lockobj)
			{
				if (!Directory.Exists(directory))
					Directory.CreateDirectory(directory);

				XmlSerializer formatter = new XmlSerializer(typeof(Settings));
				using (FileStream fs = new FileStream(file, FileMode.Create))
					formatter.Serialize(fs, Settings);
			}
		}

		public static Guid[] GetAllChat()
		{
			//return new long[] { };
			//List<long> res = new List<long>(); todo раскоментить
			return Directory.GetDirectories(Path).Select(x => x.Split("\\").LastOrDefault())
				.Where(x => Guid.TryParse(x, out var y))
				.Select(Guid.Parse)
				.ToArray();
		}
	}
}
