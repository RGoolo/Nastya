using System.Text;
using BotModel.Bots.BotTypes.Attribute;
using BotModel.Bots.BotTypes.Class;
using BotModel.Bots.BotTypes.Enums;
using Model.Settings;

namespace NightGameBot.Commands.Game
{

	[CommandClass(nameof(DzzzrSetting), "Насторойки для дозора:", TypeUser.User)]
	public class DzzzrSetting
	{
		[Command(Const.DzrGame.Prefix, "Префикс кодов, для дозора")]
		public string Prefix { get; set; }

		[Command(Const.DzrGame.CheckOtherTask, "Отслеживать изменение всех заданий, по умолчанию будет только верхнее")]
		public bool CheckOtherTask { get; set; }

		
		[Command(nameof(Start), "Стартовые данные для дозора.", TypeUser.User)]
		public IMessageToBot Start()
		{
			var sb = new StringBuilder();
			
			sb.AppendLine("Для игры в дозор скопируйте текст и вставте свои значения. Если начать предложение с \"!\" бьет в спойлер.");

			sb.AppendLine($"/{Const.Game.Site} http://classic.dzzzr.ru/demo/");
			sb.AppendLine($"/{Const.Game.Login} login");
			sb.AppendLine($"/{Const.Game.Password} \"password\"");
			sb.AppendLine($"/{Const.DzrGame.Prefix} - префикс игры");
			sb.AppendLine($"/{Const.Game.Send}_on - включить отправку кодов");
			sb.AppendLine($@"/{Const.Game.CopyFromPM} - Скопировать логин\пароль\сайт из лички");

			return MessageToBot.GetTextMsg(sb.ToString());
		}
	}
}