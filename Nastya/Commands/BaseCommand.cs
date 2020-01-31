using System;
using Model.BotTypes.Class;
using Model.BotTypes.Enums;

namespace Nastya.Commands
{
	public abstract class BaseCommand
	{
		public ISendMessages SendMsg { get; set; }
		// public TypeBot TypeBot { get; set; }
		// public IChatId ChatId { get; set; }
		// protected IChatFileFactory FileWorker => SettingsHelper.GetSetting(ChatId).FileChatWorker;
	}
}
