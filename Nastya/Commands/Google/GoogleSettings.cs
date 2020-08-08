using Model.Bots.BotTypes.Attribute;
using Model.Bots.BotTypes.Enums;
using Model.Bots.BotTypes.Interfaces.Ids;
using Model.Bots.BotTypes.Interfaces.Messages;
using Model.Files.FileTokens;
using Model.Logic.Settings;

namespace Nastya.Commands.Google
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
				userFiles.SystemFile(SystemChatFile.GoogleToken)
					.CopyFrom(settings.FileChatFactory.SystemFile(SystemChatFile.GoogleToken));
				userFiles.SystemFile(SystemChatFile.RecognizeCredentials)
					.CopyFrom(settings.FileChatFactory.SystemFile(SystemChatFile.RecognizeCredentials));

				settings.Coordinates.GoogleCred = userSettings.Coordinates.GoogleCred; //ToDo: move to coordinate
			}

			return "Успешно скопировано";
		}

        [Command(nameof(SaveTokenRecognizer), "Файл с токеном для доступа к excel странице.", TypeUser.User, TypeResource.Document)]
        public string SaveTokenRecognizer(IBotMessage msg, IChatFile fileToken, IChatFileFactory factory)
        {
            factory.SystemFile(SystemChatFile.RecognizeCredentials).CopyFrom(fileToken);
            fileToken.Delete();
            return "Успешно получено";
        }

        [Command(nameof(SaveTokenSheet), "Файл с токеном для доступа к сервисам распознования текста.", TypeUser.User, TypeResource.Document)]
        public string SaveTokenSheet(IBotMessage msg, IChatFile fileToken, IChatFileFactory factory)
        {
            factory.SystemFile(SystemChatFile.SheetCredentials).CopyFrom(fileToken);
            fileToken.Delete();
            return "Успешно получено";
        }

		[Command(nameof(SaveGoogleToken), "Google.Apis.Auth.OAuth2.Responses.TokenResponse-user. Файл с токеном для авторизации к excel странице.", TypeUser.User, TypeResource.Document)]
        public string SaveGoogleToken(IBotMessage msg, IChatFile fileToken, IChatFileFactory factory)
        {
            factory.SystemFile(SystemChatFile.GoogleToken).CopyFrom(fileToken);
            fileToken.Delete();
            return "Успешно получено";
        }

        [Password]
        [Command(Const.Coordinates.Google.GoogleCred, "Ключ к доступам апи карт гугла")]
        public string GoogleCred { get; set; }
	}
}