using System.Text;
using Model.Bots.BotTypes.Attribute;
using Model.Bots.BotTypes.Enums;
using Model.Bots.BotTypes.Interfaces.Ids;
using Model.Bots.BotTypes.Interfaces.Messages;
using Model.Logic.Settings;

namespace Nastya.Commands
{
	[CommandClass("SystemCommand", "Системные настройки.",  TypeUser.User)]
	class SystemCommand
	{
		[Command(nameof(WithoutPrefix), "Реагировать на комманды без префикса бота.")]
		public bool WithoutPrefix { get; set; }

		[Command(nameof(Clear), "Очистить настройки.")]
		public string Clear(IChatId chatId)
		{
			SettingsHelper.GetSetting(chatId).Clear();
			return "Настройки сброшены";
		}

		[Command(nameof(Me), "Очистить настройки.")]
		public string Me(IChatId chatId)
		{
			return chatId.ToString();
		}


		[Command(nameof(Me), "Очистить настройки.")]
		public string Me(IChatId chatId, IUser user)
		{
			var sb = new StringBuilder();
			sb.AppendLine($"id: {user.Id}");
			sb.AppendLine($"Display name: {user.Display}");
			sb.AppendLine($"Type: {user.Type}");
			return sb.ToString();
		}
	}
}
