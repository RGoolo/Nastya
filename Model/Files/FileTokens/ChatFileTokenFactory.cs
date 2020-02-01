using System;
using System.IO;
using Model.BotTypes.Class;
using Model.BotTypes.Enums;

namespace Model.Files.FileTokens
{
	public interface IChatFileFactory
	{
		IChatFile NewResourcesFileTokenByExt(string ext);
		IFile  NewResourcesFileByExt(string ext);
		IChatFile SettingsFile();
		IChatFile InternetFile(string uri);
		IChatFile GetExistFileByPath(string fullFileName);
		IChatFile GetChatFile(IFileToken fileToken);
	}

	public static class UriHelper
	{
		public static string GetFileName(string uri)
		{
			var index = uri.LastIndexOf('/');
			return index == -1 ? uri : uri.Substring(index + 1);
		}
	}

	public class ChatFileTokenFactory : IChatFileFactory
	{
		private string _botSettingsPath;
		private readonly IChatId _chatId;
		private const string SettingsFileName = "setting.xml";
		private const string ResourcesFolder = "resources";
		public ChatFileTokenFactory(IChatId chatId, string botSettingsPath)
		{
			_botSettingsPath = botSettingsPath;
			_chatId = chatId;
		}

		public IChatFile NewResourcesFileTokenByExt(string ext)
		{
			var fileName = Guid.NewGuid() + ext; // Какой шанс что гуиды повторятся?
			var fullName = Path.Combine(_botSettingsPath, ResourcesFolder, fileName);

			return new ChatFile(
				FileTypeFlags.IsChat | FileTypeFlags.IsLocal | FileTypeFlags.Resources,
				fileName,
				_chatId,
				fullName);
		}


		public IFile NewResourcesFileByExt(string ext)
		{
			throw new NotImplementedException();
		}

		public IChatFile SettingsFile()
		{
			var fileName = SettingsFileName;
			var fullName = Path.Combine(_botSettingsPath, fileName);
			var res = Path.Combine(_botSettingsPath, ResourcesFolder);

			if (!Directory.Exists(res))
				Directory.CreateDirectory(res);

			return new ChatFile(FileTypeFlags.IsChat | FileTypeFlags.IsLocal | FileTypeFlags.Settings,
				SettingsFileName,
				_chatId,
				fullName);
		}

		public IChatFile InternetFile(string uri)
		{
			var name = UriHelper.GetFileName(uri);
			return new ChatFile(FileTypeFlags.IsChat, name, _chatId, uri);
		}

		public IChatFile GetChatFile(IFileToken fileToken)
		{
			return fileToken.IsLocal() ? GetExistFileByPath(fileToken.Location) : InternetFile(fileToken.Location);
		}

		public IChatFile GetExistFileByPath(string fullFileName)
		{
			return new ChatFile(FileTypeFlags.IsChat | FileTypeFlags.IsCustomFile, Path.GetFileName(fullFileName),
				_chatId, fullFileName);
		}
	}
}