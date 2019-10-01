using Model.Logic.Settings;
using Model.Types.Attribute;
using Model.Types.Class;
using Model.Types.Enums;
using Telegram.Bot.Types;
using Web.DL;

namespace Nastya.Commands
{
	
	[CommandClass(nameof(SheetsCommands), "Всякое барахло для тестов. ToDo: Убрать это из хелпа!", Model.Types.Enums.TypeUser.User)]
	public class TestCommand
	{

		[Command(nameof(GetNotification), "Получаем нотификашку тового левала", typeUser: TypeUser.User)]
		public TransactionCommandMessage GetNotification(string name)
		{
			var dlPage = new DLPage();
			dlPage.LevelNumber = "1";
			dlPage.LevelTitle = "title1";
			dlPage.Html = "test html 1";

			var dlPage2 = new DLPage();
			dlPage2.LevelNumber = "2";
			dlPage2.LevelTitle = "title1";
			dlPage2.Html = "test html 2";


			var message = CommandMessage.GetTextMsg(new Texter(name, true));
			message.Notification = Notification.NewLevel;
			message.NotificationObject = new[] {dlPage, dlPage2};
			return new TransactionCommandMessage(message);

		}
	}
}