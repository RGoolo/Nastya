using System.Collections.Generic;
using System.Linq;
using System.Text;
using Google.Rpc;
using Model.BotTypes.Attribute;
using Model.BotTypes.Class;
using Model.BotTypes.Enums;
using Model.Logic.PereodicTable;
using Model.Logic.Settings;

namespace Nastya.Commands
{

	[CommandClass(nameof(DlSettings), "Насторойки для дедлайна:", TypeUser.User)]
	public class DlSettings
	{
		[Command(nameof(Const.DlGame.Sturm), "Переводит строку в символы елементов.", TypeUser.User)]
		public string TimeFormat { get; set; }

		[Command(Const.DlGame.Sturm, "Если игра штурмавая, не забывайте указать уровень. /" + Const.Game.Level)]
		public bool Sturm { get; set; }

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