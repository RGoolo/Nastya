using System;
using System.Collections.Generic;
using Model.BotTypes.Class;
using Model.BotTypes.Class.Ids;
using Model.BotTypes.Interfaces.Messages;
using Model.Files.FileTokens;

namespace Model.Logic.Settings
{
	public static class SettingsHelper
	{
		//ToDo: in DB
		public static readonly Dictionary<IChatId, ISettings> Settings = new Dictionary<IChatId, ISettings>();

		public static ISettings GetSetting(IUser user)
		{
			return GetSetting(new ChatGuid(user.Id));
		}

		public static ISettings GetSetting(IChatId chatId)
		{
			if (!Settings.ContainsKey(chatId))
				Settings.Add(chatId, new SettingHelper(chatId, System.IO.Path.Combine(Directory, chatId.GetId.ToString())));

			return Settings[chatId];
		}

		public static string Directory { get; set; }

		// public static IChatFileWorker FileWorker => _fileWorker ??= new LocalChatFileWorker(new ChatGuid(Guid.Empty));
	}
}