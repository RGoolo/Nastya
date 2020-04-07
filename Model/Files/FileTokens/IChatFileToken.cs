using System;
using Model.Bots.BotTypes.Enums;
using Telegram.Bot.Types;

namespace Model.Files.FileTokens
{
	public interface IChatFileToken
	{
		FileTypeFlags FileType { get; }
		string FileName { get; }
		string Location { get; }
	}
}