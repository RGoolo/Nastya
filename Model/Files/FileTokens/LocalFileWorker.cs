using System;
using System.IO;
using System.Xml.Serialization;
using Model.BotTypes.Class;

namespace Model.Files.FileTokens
{
	/*
	public class DirectoryFileWorker : IChatFileWorker
	{
		private static string LocalPath => @"botSetting"; //ToDo fromSettings
		private readonly string _directory;

		private string GetFullPath(string fileName) => Path.Combine(_directory, fileName);

		private string GetUnicalNameByExt(string ext)
		{
			string file;

			do { file = Path.Combine(_directory, Guid.NewGuid() + ext); }
			while (System.IO.File.Exists(file));

			return file;
		}

		public static DirectoryFileWorker Create() => new DirectoryFileWorker(LocalPath);
		public static DirectoryFileWorker Create(string subFolder) => new DirectoryFileWorker(Path.Combine(LocalPath, subFolder));
		public static DirectoryFileWorker CreateCustomDirectory(string directory) => new DirectoryFileWorker(directory);

		private DirectoryFileWorker(string directory)
		{
			_directory = directory;
		}

		public FileStream ReadStream(IFileToken token)
		{
			throw new NotImplementedException();
		}

		public FileStream WriteStream(IFileToken token)
		{
			throw new NotImplementedException();
		}

		public string ReadToEnd(IFileToken token)
		{
			throw new NotImplementedException();
		}

		public void SaveObject<T>(T instance, IFileToken token)
		{
			var file = GetFullPath(token.FileName);

			//lock (_lockobj)
			{
				var formatter = new XmlSerializer(typeof(T));
				using FileStream fs = new FileStream(file, FileMode.Create);
				formatter.Serialize(fs, instance);
			}
		}
	}

	public class LocalChatFileWorker : IChatFileWorker
	{
		private readonly IChatId _chatId;
		private static string LocalPath => @"botSetting"; //ToDo fromSettings
		private static string folderForSettings => @"setting"; //ToDo fromSettings

		private readonly string _directory;
		
		private string GetFullPath(string fileName) => Path.Combine(_directory, fileName);
		private string GetUnicNameByExt(string ext)
		{
			string file;
			
			do { file = Path.Combine(_directory, Guid.NewGuid() + ext);} 
			while (System.IO.File.Exists(file));
			
			return file;
		}

		public LocalChatFileWorker(IChatId chatId)
		{
			_chatId = chatId;
			var directory = Path.Combine(LocalPath, chatId.ToString());
			if (!Directory.Exists(directory))
				Directory.CreateDirectory(directory);

			_directory = directory;
		}

		public void SaveObject<T>(T instance, IChatFileToken token)
		{
			var file = Path.Combine(_directory, token.FileName);

			//lock (_lockobj)
			{
				var formatter = new XmlSerializer(typeof(T));
				using FileStream fs = new FileStream(file, FileMode.Create);
				formatter.Serialize(fs, instance);
			}
		}

		public FileStream ReadStream(IChatFileToken token)
		{
			if (!token.FileType.IsLocal())
				throw new NotImplementedException();

			return System.IO.File.OpenRead(GetFullPath(token.FileName));
		}

		public FileStream WriteStream(IChatFileToken token)
		{
			if (!token.FileType.IsLocal())
				throw new NotImplementedException();

			// Console.WriteLine("localToken.FilePath:" + localToken.FileFullPath);
			return new FileStream(GetFullPath(token.FileName), FileMode.Create);
		}
	/*		public IChatFileToken NewResourcesFileTokenByExt(string ext) => new LocalChatFileToken(GetUnicNameByExt(ext), _chatId);

		public IChatFileToken NewFileToken(string fileName) => new LocalChatFileToken(Path.Combine(_directory, fileName), _chatId);

		public IChatFileToken NewFileTokenByPath(string fileName) => new LocalChatFileToken(fileName, _chatId);

		public IChatFileToken SaveFile(FileStream stream) => throw new NotImplementedException();

		public string ReadToEnd(IChatFileToken token) => throw new NotImplementedException();

		public IChatFileToken GetExistFileByName(string fileName) => new LocalChatFileToken(Path.Combine(_directory, fileName), _chatId);

		public IChatFileToken GetExistFileByPath(string fullName) => new LocalChatFileToken(fullName, _chatId);
		public FileStream ReadStream(IFileToken token)
		{
			throw new NotImplementedException();
		}

		public FileStream WriteStream(IFileToken token)
		{
			throw new NotImplementedException();
		}

		public string ReadToEnd(IFileToken token)
		{
			throw new NotImplementedException();
		}

		public void SaveObject<T>(T instance, IFileToken token)
		{
			throw new NotImplementedException();
		}

		public IChatFile NewResourcesFileTokenByExt(string jpg)
		{
			throw new NotImplementedException();
		}
	}*/
}