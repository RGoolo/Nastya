
using Model.Logic.Google;
using Model.Logic.Settings;
using Model.Types.Attribute;
using Model.Types.Enums;
using Model.Types.Interfaces;
using Telegram.Bot.Types;
using Web.DL;

namespace Nastya.Commands
{
	
	[CommandClass(nameof(SheetsCommands), "Работа с Google Sheets.", Model.Types.Enums.TypeUser.User)]
	public class SheetsCommands
	{

		private readonly WorkerSheets _workerSheets = new WorkerSheets();

		[Command(nameof(CreateSheetUp), "Создает страницу в Google sheets когда появляется новый уровень.")]
		public bool CreateSheetUp { get; set; }

		[Command(nameof(SheetsUrl), "Ссылка на документ")]
		public string SheetsUrl
		{
			get => _workerSheets.SheetToken;
			set
			{
				_workerSheets.SheetToken = value.Contains("/") ? value.Split("/")[5] : value;
			}
		}

		[Command(nameof(CreateSheet), "Создать документ с именем.", typeUser: TypeUser.User)]
		public void CreateSheet(string namePage)
		{
			 _workerSheets.CreateSheetsAsync(namePage);
		}


		[CommandOnMsg(nameof(CreateSheetUp), Model.Types.Enums.MessageType.SystemMessage, typeUser: TypeUser.User)]
		public void Notifications(Model.Types.Interfaces.IMessage msg)
		{
			if (msg.ReplyToCommandMessage?.Notification != Notification.NewLevel)
				return;
			
			var dlPages = (DLPage[]) (msg.ReplyToCommandMessage?.NotificationObject);
			var lastPage = dlPages[0];
			var newPage = dlPages[1];

			var pageName = newPage.LevelNumber + newPage.LevelTitle;
			_workerSheets.CreateSheetsAsync(newPage.LevelNumber + newPage.LevelTitle);
			_workerSheets.UpdateDlPage(pageName, "A100", newPage.Html);

			pageName = lastPage.LevelNumber + lastPage.LevelTitle;
			_workerSheets.CreateSheetsAsync(lastPage.LevelNumber + lastPage.LevelTitle);
			_workerSheets.UpdateDlPage(pageName, "A101", lastPage.Html);
			
		}
	}
}