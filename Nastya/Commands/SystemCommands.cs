using System.Text;
using BotModel.Bots.BotTypes.Attribute;
using BotModel.Bots.BotTypes.Enums;
using BotModel.Bots.BotTypes.Interfaces.Ids;
using BotModel.Bots.BotTypes.Interfaces.Messages;
using BotModel.Settings;
using Model.Settings;

namespace NightGameBot.Commands
{
	[CommandClass("SystemCommand", "Системные настройки.",  TypeUser.User)]
	class SystemCommand
	{
		[Command(nameof(WithoutPrefix), "Реагировать на комманды без префикса бота.")]
		public bool WithoutPrefix { get; set; }

		[Command(nameof(Clear), "Очистить настройки.")]
		public string Clear(IChatId chatId)
		{
			SettingsHelper<SettingHelper>.GetSetting(chatId).Clear();
			return "Настройки сброшены";
		}

		[Command(nameof(Chat), "Инфо по чату.")]
		public string Chat(IChatId chatId)
		{
			return chatId.ToString();
		}

		[Command(nameof(Me), "Инфо по тебе")]
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
