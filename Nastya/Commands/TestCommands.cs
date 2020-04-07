using Model.Bots.BotTypes.Attribute;
using Model.Bots.BotTypes.Class;
using Model.Bots.BotTypes.Enums;
using Web.DL;

namespace Nastya.Commands
{
	
	[CommandClass(nameof(TestCommand), "Всякое барахло для тестов", TypeUser.Developer)]
	public class TestCommand
	{

		[Command(nameof(GetNotification), "Получаем нотификашку тового левала", typeUser: TypeUser.Developer)]
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


			var message = MessageToBot.GetTextMsg(new Texter(name, true));
			message.Notification = Notification.NewLevel;
			message.NotificationObject = new[] {dlPage, dlPage2};
			return new TransactionCommandMessage(message);

		}
	}
}