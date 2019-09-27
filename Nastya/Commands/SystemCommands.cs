using Model.Logic.Settings;
using Model.Types.Attribute;

namespace Nastya.Commands
{
	[CommandClass("SystemCommand", "Системные настройки.",  Model.Types.Enums.TypeUser.User)]
	class SystemCommand : BaseCommand
	{
		[Command(nameof(WithoutPrefix), "Реагировать на комманды без префикса бота.")]
		public bool WithoutPrefix { get; set; }

		[Command(nameof(Clear), "Очистить настройки.")]
		public string Clear()
		{
			SettingsHelper.GetSetting(ChatId).Clear();
			return "Настройки сброшены";
		}

		[Command(nameof(Me), "Очистить настройки.")]
		public string Me()
		{
			return ChatId.ToString();
		}
	}
}
