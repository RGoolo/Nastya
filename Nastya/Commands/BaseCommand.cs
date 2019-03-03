using System;
using Model.Logic.Settings;
using Model.Types.Class;
using Model.Types.Enums;
using Model.Types.Interfaces;

namespace Nastya.Commands
{
	public abstract class BaseCommand
	{
		public virtual event SendMsgDel SendMsg;

		public TypeBot TypeBot { get; set; }
		public Guid ChatId { get; set; }
		protected IChatFileWorker FileWorker => SettingsHelper.GetSetting(ChatId).FileWorker;
	}

	public delegate void SendMsgDel(TransactionCommandMessage tMessage);
}
