using System.Text;
using Model.Bots.BotTypes.Attribute;
using Model.Bots.BotTypes.Class;
using Model.Bots.BotTypes.Enums;
using Model.Logic.Settings;

namespace Nastya.Commands.Game
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