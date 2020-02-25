using System;
using Model.BotTypes.Attribute;
using Model.BotTypes.Class;
using Model.BotTypes.Class.Ids;
using Model.BotTypes.Enums;
using Model.BotTypes.Interfaces.Messages;
using Model.Files.FileTokens;
using Model.Logic.Settings;

namespace Nastya.Commands
{



	[CommandClass("GoogleSettings", "google creads", TypeUser.User)]
	public class GoogleSettings
	{
		[Command(nameof(CopyCreadFromPM), "Скопировать файл с токеном для доступа из лички", TypeUser.Admin)]
		public string CopyCreadFromPM(IUser user, ISettings settings, IChatId chatId)
		{
			if (user.Id != chatId.GetId)
			{
				var userSettings =  SettingsHelper.GetSetting(user);
				var userFiles = userSettings.FileChatFactory;

				userFiles.SystemFile(SystemChatFile.SheetCredentials)
					.CopyFrom(settings.FileChatFactory.SystemFile(SystemChatFile.SheetCredentials));
				userFiles.SystemFile(SystemChatFile.SheetToken)
					.CopyFrom(settings.FileChatFactory.SystemFile(SystemChatFile.SheetToken));
				userFiles.SystemFile(SystemChatFile.RecognizeCredentials)
					.CopyFrom(settings.FileChatFactory.SystemFile(SystemChatFile.RecognizeCredentials));

				settings.Coordinates.GoogleCreads = userSettings.Coordinates.GoogleCreads; //ToDo: move to coordinate
			}

			return "Успешно скопировано";
		}

		[Command(nameof(SaveTokenRecognizer), "Файл с токеном для доступа к странице", TypeUser.User, TypeResource.Document)]
		public string SaveTokenRecognizer(IBotMessage msg, IChatFile fileToken, IChatFileFactory factory)
		{
			factory.SystemFile(SystemChatFile.RecognizeCredentials).CopyFrom(fileToken);
			fileToken.Delete();
			return "Успешно получено";
		}
	}
}