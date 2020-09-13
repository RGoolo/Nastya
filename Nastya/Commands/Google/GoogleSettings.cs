using System;
using System.Collections.Generic;
using BotModel.Bots.BotTypes.Attribute;
using BotModel.Bots.BotTypes.Enums;
using BotModel.Bots.BotTypes.Interfaces.Ids;
using BotModel.Bots.BotTypes.Interfaces.Messages;
using BotModel.Files.FileTokens;
using BotModel.Settings;
using Model;
using Model.Settings;

namespace NightGameBot.Commands.Google
{



	[CommandClass("GoogleSettings", "google creads", TypeUser.User)]
    public class GoogleSettings
    {
        [Command(nameof(CopyCreadFromPM), "Скопировать файл с токеном для доступа из лички", TypeUser.Admin)]

        public List<string> CopyCreadFromPM(IUser user, IChatService settings, IChatId chatId)
        {
            var result = new List<string>();
            if (user.Id != chatId.GetId)
            {
                var userSettings = SettingsHelper.GetChatService(user);
                result.Add(TryCopy(user, chatId, SystemChatFile.SheetCredentials));
                result.Add(TryCopy(user, chatId, SystemChatFile.GoogleToken));
                result.Add(TryCopy(user, chatId, SystemChatFile.RecognizeCredentials));

                settings.Coordinates.GoogleCred = userSettings.Coordinates.GoogleCred; //ToDo: move to coordinate
                result.Add($"Скопировано {settings.Coordinates.GoogleCred}");
            }
            else
                result.Add("Копирование возможно только в чаты");

            return result;
        }

        private string TryCopy(IUser user, IChatId chatId, SystemChatFile chatFileType)
        {
            try
            {
                Copy(user, chatId, chatFileType);
                return $"{chatFileType}: скопировано";
            }
            catch (Exception e)
            {
                //ToDo log
                Console.WriteLine(e);
            }

            return $"{chatFileType}: ошибка при копировании";
        }

        private void Copy(IUser user, IChatId chatId, SystemChatFile chatFileType)
        {
            var userSettings = SettingsHelper.GetChatService(user);
            var userFiles = userSettings.FileChatFactory;

            var chatSettings = SettingsHelper.GetChatService(chatId);
            var chatFiles = chatSettings.FileChatFactory;

            var chatFile = chatFiles.SystemFile(chatFileType);
            var userFile = userFiles.SystemFile(chatFileType);

            chatFile.CopyFrom(userFile);
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