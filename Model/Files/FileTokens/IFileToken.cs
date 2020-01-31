using System;
using System.IO;
using Model.BotTypes.Class;
using Model.BotTypes.Enums;
using Telegram.Bot.Types;

namespace Model.Files.FileTokens
{

	public interface IChatFileId
	{
		IChatId ChatId { get; }
	}


	public interface IChatFileToken : IFileToken, IChatFileId
	{
		
	}

	public interface IChatFile : IFile, IChatFileId
	{

	}

	public interface IFile : IFileToken
	{
		FileStream ReadStream();
		FileStream WriteStream();
	}

	public interface IFileToken
	{
		FileTypeFlags FileType { get; }
		string FileName { get; }
		string Location { get; }
	}
}