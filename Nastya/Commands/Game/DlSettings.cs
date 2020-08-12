using System.Text;
using BotModel.Bots.BotTypes.Attribute;
using BotModel.Bots.BotTypes.Class;
using BotModel.Bots.BotTypes.Enums;
using Model.Settings;

namespace NightGameBot.Commands.Game
{

	[CommandClass(nameof(DlSettings), "Насторойки для дедлайна:", TypeUser.User)]
	public class DlSettings
	{
		[Command(nameof(Start), "Стартовые данные для дедлайна.", TypeUser.User)]
		public IMessageToBot Start()
		{
			var sb = new StringBuilder();
			
			sb.AppendLine("Для игры в Deadline скопируйте текст и вставте свои значения:");
			// sb.AppendLine($"/{Const.DlGame.Sturm}_on - переключает игру в штурмовой режим. Что бы выключить нажмите: /{Const.DlGame.Sturm}_off");

			sb.AppendLine($"/{Const.Game.Site} http://m.deadline.en.cx/GameDetails.aspx?gid=");
			sb.AppendLine($"/{Const.Game.Login} login");
			sb.AppendLine($"/{Const.Game.Password} \"password\"");
			sb.AppendLine($"/{Const.Game.Send}_on - включить отправку кодов");
			sb.AppendLine($@"/{Const.Game.CopyFromPM} - Скопировать логин\пароль\сайт из лички");
			
			return MessageToBot.GetTextMsg(sb.ToString());
		}
	}
}