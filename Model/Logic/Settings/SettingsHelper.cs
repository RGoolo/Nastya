using Model.Types.Interfaces;
using System;
using System.Collections.Generic;

namespace Model.Logic.Settings
{
	public static class SettingsHelper
	{
		//ToDo: in DB
		public static readonly Dictionary<Guid, ISettings> Settings = new Dictionary<Guid, ISettings>();
		private static IFileWorker _fileWorker;

		public static ISettings GetSetting(Guid chatId)
		{
			if (!Settings.ContainsKey(chatId))
				Settings.Add(chatId, new SettingHelper(chatId));

			return Settings[chatId];
		}

		public static IFileWorker FileWorker => _fileWorker ?? ( _fileWorker = new LocalFileWorker(Guid.Empty));
	}
}