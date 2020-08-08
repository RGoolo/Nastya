using System.ComponentModel;
using System.Threading.Tasks;
using Model.Bots.BotTypes.Attribute;
using Model.Bots.BotTypes.Enums;
using Model.Bots.BotTypes.Interfaces.Messages;
using Model.Files.FileTokens;
using Model.Logic.Google;
using Model.Logic.Model;
using Model.Logic.Settings;
using Web.DL;

namespace Nastya.Commands.Google
{
	
	[CommandClass(nameof(SheetsCommands), "Работа с Google Sheets.", TypeUser.User)]
	public class SheetsCommands
	{
		private WorkerSheets _workerSheets;
		private string _sheetsUrl;
		private readonly IChatFileFactory _fileFactory;

		public SheetsCommands(ISettings settings)
		{
			_fileFactory = settings.FileChatFactory;
		}

		private WorkerSheets Create(string url)
		{
			if (string.IsNullOrEmpty(url))
				throw new ModelException("Не задан путь к документу.");

			var fileCred = _fileFactory.SystemFile(SystemChatFile.SheetCredentials);
			if (!fileCred.Exists())
				throw new ModelException("Не задан Token доступа.");

			var fileToken = _fileFactory.SystemFile(SystemChatFile.GoogleToken);
			return new WorkerSheets(fileCred, fileToken, url);
		}

		private WorkerSheets GetSheets()
		{
			return _workerSheets ??= Create(_sheetsUrl?.Contains("/") == true ? _sheetsUrl.Split("/")[5] : _sheetsUrl);
		}

		private void RecreateSheets(string url)
		{
			_sheetsUrl = url?.Contains("/") == true ? url.Split("/")[5] : url;
			if (_workerSheets == null) return;
			_workerSheets = Create(_sheetsUrl);
		}

		[Command(nameof(CreateSheetUp), "Создает страницу в Google sheets когда появляется новый уровень.")]
		public bool CreateSheetUp { get; set; }

		[Command(nameof(SheetsUrl), "Ссылка на документ")]
		public string SheetsUrl 
		{
			get =>_sheetsUrl;
			set => RecreateSheets(value);
		}

		[Command(nameof(CreateSheet), "Создать документ с именем.", typeUser: TypeUser.User)]
		public Task CreateSheet([Description("Имя создаваемой страницы")] string namePage)
		{
			return GetSheets().CreateSheetsAsync(namePage);
		}

		[Command(nameof(LvlCell), "Ячейка для записи html первой страниц")]
		public string LvlCell { get; set; } = "A100";

		[Command(nameof(NewLvlCell), "Ячейка для записи html последней страниц")]
		public string NewLvlCell { get; set; } = "A101";


		[CommandOnMsg(nameof(CreateSheetUp), MessageType.All, typeUser: TypeUser.Bot)]
		public async Task Notifications(IBotMessage msg)
		{
			if (msg.ReplyToCommandMessage?.Notification != Notification.NewLevel)
				return;
			
			var dlPages = (DLPage[]) (msg.ReplyToCommandMessage?.NotificationObject);
			var lastPage = dlPages[0];
			var newPage = dlPages[1];

			var pageName = newPage.LevelNumber + newPage.LevelTitle;
			await GetSheets().CreateSheetsAsync(newPage.LevelNumber + newPage.LevelTitle);
			await GetSheets().UpdateDlPage(pageName, LvlCell, newPage.Html);

			pageName = lastPage.LevelNumber + lastPage.LevelTitle;
			await GetSheets().CreateSheetsAsync(lastPage.LevelNumber + lastPage.LevelTitle);
			await GetSheets().UpdateDlPage(pageName, NewLvlCell, lastPage.Html);
		}
	}
}