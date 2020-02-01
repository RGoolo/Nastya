
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.Utilities.Helpers.Interfaces;
using Model.BotTypes.Attribute;
using Model.BotTypes.Class;
using Model.BotTypes.Class.Ids;
using Model.BotTypes.Enums;
using Model.BotTypes.Interfaces;
using Model.BotTypes.Interfaces.Messages;
using Model.Files.FileTokens;
using Model.Logic.Google;
using Model.Logic.Model;
using Model.Logic.Settings;
using Web.DL;

namespace Nastya.Commands
{
	
	[CommandClass(nameof(SheetsCommands), "Работа с Google Sheets.", TypeUser.User)]
	public class SheetsCommands : BaseCommand
	{
		private WorkerSheets _workerSheets;
		private string _sheetsUrl;

		private static WorkerSheets Create(IChatFileFactory fileFactory, string url)
		{
			if (string.IsNullOrEmpty(url))
				throw new ModelException("Не задан путь к документу.");

			var fileCred = fileFactory.SystemFile(SystemChatFile.SheetCredentials);
			if (!fileCred.Exists())
				throw new ModelException("Не задан Token доступа.");

			var fileToken = fileFactory.SystemFile(SystemChatFile.SheetToken);
			return new WorkerSheets(fileCred, fileToken, url);
		}

		private WorkerSheets GetSheets(IChatFileFactory fileFactory)
		{
			return _workerSheets ??= Create(fileFactory, _sheetsUrl?.Contains("/") == true ? _sheetsUrl.Split("/")[5] : _sheetsUrl);
		}

		private void RecreateSheets(IChatFileFactory fileFactory)
		{
			if (_workerSheets == null) return;
			_workerSheets = Create(fileFactory, _sheetsUrl?.Contains("/") == true ? _sheetsUrl.Split("/")[5] : _sheetsUrl);
		}

		[Command(nameof(SaveTokenSheet), "Файл с токеном для доступа к странице", TypeUser.User, TypeResource.Document)]
		public string SaveTokenSheet(IBotMessage msg, IChatFileFactory fileFactory, IChatFile fileToken)
		{
			fileFactory.SystemFile(SystemChatFile.SheetCredentials).CopyFrom(fileToken);
			fileToken.Delete();
			return "Усешно получено";
		}

		[Command(nameof(CopyCreadFromPM), "Скопировать файл с токеном для доступа из лички", TypeUser.Admin)]
		public string CopyCreadFromPM(IUser user, IChatFileFactory factory, IChatId chatId)
		{
			if (user.Id != chatId.GetId)
			{
				var userFiles = SettingsHelper.GetSetting(user).FileChatFactory;
				userFiles.SystemFile(SystemChatFile.SheetCredentials)
					.CopyFrom(factory.SystemFile(SystemChatFile.SheetCredentials));
				userFiles.SystemFile(SystemChatFile.SheetToken).CopyFrom(factory.SystemFile(SystemChatFile.SheetToken));
			}

			RecreateSheets(factory);
			return "Успешно скопировано";
		}

		[Command(nameof(CreateSheetUp), "Создает страницу в Google sheets когда появляется новый уровень.")]
		public bool CreateSheetUp { get; set; }

		[Command(nameof(SheetsUrl), "Ссылка на документ")]
		public string SheetsUrl(IChatFileFactory fileFactory, [Description("ссылка на документ")] string url)
		{
			_sheetsUrl = url;
			RecreateSheets(fileFactory);
			return nameof(SheetsUrl) + "=" + url;
		}

		[Command(nameof(CreateSheet), "Создать документ с именем.", typeUser: TypeUser.User)]
		public Task CreateSheet([Description("Имя создаваемой страницы")] string namePage, IChatFileFactory fileFactory)
		{
			return GetSheets(fileFactory).CreateSheetsAsync(namePage);
		}

		[Command(nameof(LvlCell), "Ячейка для записи html первой страниц")]
		public string LvlCell { get; set; } = "A100";

		[Command(nameof(NewLvlCell), "Ячейка для записи html последней страниц")]
		public string NewLvlCell { get; set; } = "A101";


		[CommandOnMsg(nameof(CreateSheetUp), MessageType.SystemMessage, typeUser: TypeUser.User)]
		public async Task Notifications(IBotMessage msg, IChatFileFactory fileFactory)
		{
			if (msg.ReplyToCommandMessage?.Notification != Notification.NewLevel)
				return;
			
			var dlPages = (DLPage[]) (msg.ReplyToCommandMessage?.NotificationObject);
			var lastPage = dlPages[0];
			var newPage = dlPages[1];

			var pageName = newPage.LevelNumber + newPage.LevelTitle;
			await GetSheets(fileFactory).CreateSheetsAsync(newPage.LevelNumber + newPage.LevelTitle);
			await GetSheets(fileFactory).UpdateDlPage(pageName, LvlCell, newPage.Html);

			pageName = lastPage.LevelNumber + lastPage.LevelTitle;
			await GetSheets(fileFactory).CreateSheetsAsync(lastPage.LevelNumber + lastPage.LevelTitle);
			await GetSheets(fileFactory).UpdateDlPage(pageName, NewLvlCell, lastPage.Html);
		}
	}
}