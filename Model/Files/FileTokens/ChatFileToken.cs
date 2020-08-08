using System;
using System.IO;
using System.Net;
using System.Xml.Serialization;
using Model.Bots.BotTypes.Enums;
using Model.Bots.BotTypes.Interfaces.Ids;
using Model.Logic.Settings;

namespace Model.Files.FileTokens
{
	public class UrlChatFileToken : IChatFileToken
	{
		public UrlChatFileToken(string uri)
		{
			FullName = uri;
			FileType = FileTypeFlags.Resources;
			FileName = UriHelper.GetFileName(uri);
		}

		public FileTypeFlags FileType { get; }
		public string FileName { get; }
		public string FullName { get; set; }
	}


	internal class ChatFile : IChatFile
	{
		public FileTypeFlags FileType { get; }
		public string FileName { get; }


		internal ChatFile(FileTypeFlags fileType, string fileName, IChatId chatId, string location)
		{
			FileType = fileType;
			FileName = fileName;
			FullName = location;
		}
		
		/// <summary>
		/// Url or FullFilePath
		/// </summary>
		public string FullName { get; }

		public FileStream ReadStream() => ((IChatFileToken) this).ReadStream();

		public FileStream WriteStream(FileMode fileMode = FileMode.Create)
		{
			if (!FileType.IsLocal())
				throw new NotImplementedException();

			return new FileStream(FullName, FileMode.Create);
		}

		public T Read<T>()
		{
			var formatter = new XmlSerializer(typeof(T));
			using var fs = ReadStream();

			return (T)formatter.Deserialize(fs);
		}

		public void Save<T>(T type)
		{
			var formatter = new XmlSerializer(typeof(T));
			using var fs = WriteStream();

			formatter.Serialize(fs, type);
		}

		public void Delete()
		{
			if (!FileType.IsLocal())
				throw new NotImplementedException();

			File.Delete(FullName);
		}

		public bool Exists()
		{
			if (!FileType.IsLocal())
				throw new NotImplementedException();

			return File.Exists(FullName);
		}

		public void CopyFrom(IChatFileToken token)
		{
			using var write = WriteStream();
			using var read = token.ReadStream();

			read.CopyTo(write);
		}
	}
}

