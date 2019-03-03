using Model.Types.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Model.Types.Interfaces
{
	public interface IFileToken 
	{
		FileType Type { get; }
		Guid ChatId { get; }
		string FileName { get; }
		string Url { get; }
		//Guid Id { get; }
	}

	public class LocalFileToken : IFileToken
	{
		public Guid ChatId { get; }
		public string Url { get; }
		public string FileName { get; }
		public string FilePath { get; }
		public FileType Type { get; }
		//public Guid Id;

		public LocalFileToken(string filePath, Guid chatId)
		{
			ChatId = chatId;
			Type = FileType.Local;
			FilePath = filePath;
			FileName = Path.GetFileName(filePath);
		}



		//public static implicit operator InputOnlineFile(string value);
	}

	public class UrlFileToken : IFileToken
	{
		public Guid ChatId { get; }
		public string Url { get; }
		public string FileName { get; }
		public FileType Type { get; }

		public UrlFileToken(string url) : this(url, Guid.Empty) { }

		public UrlFileToken(string url, Guid chatId)
		{
			ChatId = chatId;
			Type = FileType.Uri;
			Url = url;
			FileName = url; //ToDo last.
		}
		//public static implicit operator InputOnlineFile(string value);
	}

	public interface IChatFileWorker : IFileWorker
	{
		IFileToken NewFileTokenByExt(string ext);
		IFileToken NewFileToken(string fileName);
		IFileToken GetExistFileByName(string name);

		IFileToken SaveFile(FileStream stream);
	}

	public interface IFileWorker
	{
		FileStream ReadStream(IFileToken token);
		FileStream WriteStream(IFileToken token);

		string ReadToEnd(IFileToken token);
		void SaveObject<T>(T instance, IFileToken token);
	}


	public class LocalFileWorker : IChatFileWorker
	{
		private static string LocalPath => @"D:\botseting\";

		private Guid _chatId;
		private string _directory => Path.Combine(LocalPath, _chatId.ToString());

		private string GetUnicNameByExt(string ext) =>  Path.Combine(_directory, Guid.NewGuid().ToString() + ext);

		public LocalFileWorker(Guid chatId)
		{
			_chatId = chatId;
		}

		public void SaveObject<T>(T instance, IFileToken token)
		{
			string chatId = _chatId.ToString();

			var file = Path.Combine(_directory, chatId.ToString() + ".xml");

			//lock (_lockobj)
			{
				if (!Directory.Exists(_directory))
					Directory.CreateDirectory(_directory);

				XmlSerializer formatter = new XmlSerializer(instance.GetType());
				using (FileStream fs = new FileStream(file, FileMode.Create))
					formatter.Serialize(fs, instance);
			}
		}


		public FileStream ReadStream(IFileToken token)
		{
			if (token is LocalFileToken localToken)
				return File.OpenRead(localToken.FilePath);

			throw new NotImplementedException();
		}

		public FileStream WriteStream(IFileToken token)
		{
			if (token is LocalFileToken localToken)
				return new FileStream(localToken.FilePath, FileMode.Create);

			throw new NotImplementedException();
		}

		public IFileToken NewFileTokenByExt(string ext)
		{
			return new LocalFileToken(GetUnicNameByExt(ext), _chatId);
		}

		public IFileToken NewFileToken(string fileName)
		{
			return new LocalFileToken(Path.Combine(_directory, fileName), _chatId);
			//(ext), _chatId);
		}

		public IFileToken SaveFile(FileStream stream)
		{
			throw new NotImplementedException();
		}

		public string ReadToEnd(IFileToken token)
		{
			throw new NotImplementedException();
		}

		public IFileToken GetExistFileByName(string name)
		{
			throw new NotImplementedException();
		}
	}
}
