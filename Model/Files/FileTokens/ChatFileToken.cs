using System;
using System.IO;
using System.Net;
using Model.BotTypes.Class;
using Model.BotTypes.Enums;

namespace Model.Files.FileTokens
{
	public class UrlChatFileToken : IFileToken
	{
		public UrlChatFileToken(string uri)
		{
			Location = uri;
			FileType = FileTypeFlags.Resources;
			FileName = UriHelper.GetFileName(uri); // ToDo last;
		}

		public FileTypeFlags FileType { get; }
		public string FileName { get; }
		public string Location { get; set; }
	}

	public class ChatFile : IChatFile
	{
		public FileTypeFlags FileType { get; }
		public string FileName { get; }
		public IChatId ChatId { get; set; }

		public ChatFile(FileTypeFlags fileType, string fileName, IChatId chatId, string location)
		{
			FileType = fileType;
			FileName = fileName;
			Location = location;
			ChatId = chatId;
		}
		
		/// <summary>
		/// Url or FullFilePath
		/// </summary>
		public string Location { get; }

		public FileStream ReadStream()
		{
			if (!FileType.IsLocal())
				throw new NotImplementedException();

			return System.IO.File.OpenRead(Location);
		}

		public FileStream WriteStream()
		{
			if (!FileType.IsLocal())
				throw new NotImplementedException();

			// Console.WriteLine("localToken.FilePath:" + localToken.FileFullPath);
			//var path = Path.GetFullPath(Location);
		//	if (!Directory.Exists(path))
		//		Directory.CreateDirectory(path);

			return new FileStream(Location, FileMode.Create);
		}
	}

	public class File : IFile
	{
		public FileTypeFlags FileType { get; }
		public string FileName { get; }

		public File(FileTypeFlags fileType, string fileName, string location)
		{
			FileType = fileType;
			FileName = fileName;
			Location = location;

			if (!FileType.IsLocal()) return;
			
			var path = Path.GetFullPath(Location);
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
		}

		/// <summary>
		/// Url or FullFilePath
		/// </summary>
		public string Location { get; }

		public FileStream ReadStream()
		{
			if (!FileType.IsLocal())
			{
				//WebClient wc = new WebClient();
				//return wc.DownloadData(Location);
				throw new NotImplementedException();
			}

			return System.IO.File.OpenRead(Location);
		}

		public FileStream WriteStream()
		{
			if (!FileType.IsLocal())
				throw new NotImplementedException();

			// Console.WriteLine("localToken.FilePath:" + localToken.FileFullPath);
			

			return new FileStream(Location, FileMode.Create);
		}
	}
}

