using System;
using System.Diagnostics;
using System.IO;
using Model.BotTypes.Class;
using Model.BotTypes.Enums;

namespace Model.Files.FileTokens
{
	public enum SystemChatFile
	{
		Settings, SheetToken, SheetCredentials
	}

	public interface IChatFileFactory
	{
		IChatFile NewResourcesFileByExt(string ext);
		IChatFile SystemFile(SystemChatFile type);
		IChatFile GetChatFile(IChatFileToken fileToken);
	}

	public static class UriHelper
	{
		public static string GetFileName(string uri)
		{
			var index = uri.LastIndexOf('/');
			return index == -1 ? uri : uri.Substring(index + 1);
		}
	}

	public class ChatFileFactory : IChatFileFactory
	{
		private readonly string _botSettingsPath;
		private readonly IChatId _chatId;

		private const string ResourcesFolder = "resources";
		public ChatFileFactory(IChatId chatId, string botSettingsPath)
		{
			_botSettingsPath = botSettingsPath;
			_chatId = chatId;
		}

		public IChatFile NewResourcesFileByExt(string ext)
		{
			var fileName = Guid.NewGuid() + ext; // Какой шанс что гуиды повторятся?
			var fullName = Path.Combine(_botSettingsPath, ResourcesFolder, fileName);

			return new ChatFile(
				FileTypeFlags.IsChat | FileTypeFlags.IsLocal | FileTypeFlags.Resources,
				fileName,
				_chatId,
				fullName);
		}

		public IChatFile SystemFile(SystemChatFile type)
		{
			var fileName = type switch
			{
				SystemChatFile.Settings => $"{type}.xml",
				SystemChatFile.SheetCredentials => $"{type}.json",
				SystemChatFile.SheetToken => $"Google.Apis.Auth.OAuth2.Responses.TokenResponse-user",
				_ => $"{type}.xml",
			};

			var fullName = Path.Combine(_botSettingsPath, fileName);
			var res = Path.Combine(_botSettingsPath, ResourcesFolder);

			if (!Directory.Exists(res))
				Directory.CreateDirectory(res);

			return new ChatFile(FileTypeFlags.IsChat | FileTypeFlags.IsLocal | FileTypeFlags.Settings,
				fileName,
				_chatId,
				fullName);
		}

		public IChatFile InternetFile(string uri)
		{
			var name = UriHelper.GetFileName(uri);
			return new ChatFile(FileTypeFlags.IsChat, name, _chatId, uri);
		}

		public IChatFile GetChatFile(IChatFileToken fileToken)
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