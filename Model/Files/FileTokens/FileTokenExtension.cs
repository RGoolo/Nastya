using System;
using System.IO;
using Model.Bots.BotTypes.Enums;

namespace Model.Files.FileTokens
{
	public static class FileTokenExtension
	{
		public static bool IsLocal(this FileTypeFlags flags)
		{
			return (flags & FileTypeFlags.IsLocal) != 0;
		}

		public static bool Is(this FileTypeFlags flags, FileType type) =>
			type switch
			{
				FileType.Chat => ((flags & FileTypeFlags.IsChat) != 0),
				FileType.Internet => !flags.IsLocal(),
				FileType.Local => flags.IsLocal(),
				FileType.Resources => ((flags & FileTypeFlags.Resources) != 0),
				FileType.Settings => ((flags & FileTypeFlags.Settings) != 0),
				FileType.CustomFile => ((flags & FileTypeFlags.Settings) != 0),
				_ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
			};
		
		public static bool IsLocal(this IChatFileToken flags)
		{
			return flags.FileType.IsLocal();
		}

		public static bool Is(this IChatFileToken flags, FileType type) => flags.FileType.Is(type);

		public static FileStream ReadStream(this IChatFileToken token)
		{
			if (!token.IsLocal())
				throw new NotImplementedException();

			return File.OpenRead(token.Location);
		}
	}
}