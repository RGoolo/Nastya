using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using Telegram.Bot.Types.Enums;
using System.Net;
using Model.Logic.Settings;
using Model.Types.Class;
using Model.Types.Interfaces;
using Model.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using System.Threading;
using System.Security;

namespace Model.TelegramBot
{
	internal static class WebProxyExtension
	{
		//private const string webProxyurl = "144.217.161.149:8080"; //90.187.45.5:8123";

		public static WebProxy Create()
		{
			//var proxyURI = $"{server}:{port}";
			//ICredentials credentials = new NetworkCredential(userName, password);
			//return new WebProxy(proxyURI, true, null, credentials);
			//ToDo: RandomProxy
			return new WebProxy("144.217.161.149:8080");
/*
154.117.208.214:8080
95.85.58.154:8080
103.25.122.1:8080
119.28.24.210:80
147.75.113.108:443
180.234.219.165:8080
202.150.147.22:53281
36.72.112.161:80
159.203.91.6:8080
103.210.56.209:8082
*/
		}
	}

	public class TelegramBot : BaseConcurrentBot
	{
		private class Resource : IResource
		{
			public Resource(IFileToken file, TypeResource type)
			{
				File = file;
				Type = type;
			}

			public IFileToken File { get; }
			public TypeResource Type { get; }
		}

		private readonly SecureString _token;
		private readonly CancellationToken _cancellationToken = default(CancellationToken);
		private const int UPDATETIMESTam_Minutes = 5;
		private const int MyUserId = 62779148;
		private const long MyChat = 62779148;

		private int _offset;
		private Dictionary<long, ChatAdministrations> _chatAdministations = new Dictionary<long, ChatAdministrations>();
		private TelegramHTML _telegramHtml = new TelegramHTML();
		private Telegram.Bot.TelegramBotClient _bot;

		private IChatFileWorker _chatFileWorker(Guid chatId) => SettingsHelper.GetSetting(chatId).FileWorker;
		private IFileWorker _fileWorker => SettingsHelper.FileWorker;

		public TelegramBot(SecureString token, Guid id) : base(TypeBot.Telegram, id)
		{
			_loger.WriteTrace(".ctor");
			var cred = new NetworkCredential(string.Empty, token).Password;

			_bot = new Telegram.Bot.TelegramBotClient(cred, WebProxyExtension.Create());
			//ToDo: SetWebHook 
			//ToDo: GetMeAsync
			_token = token;
		}

		private TypeUser GetTypeUser(bool isAdmin, Telegram.Bot.Types.Message msg)
		{
			TypeUser userType = TypeUser.User;

			if (msg.From.Id == MyUserId)
				userType |= TypeUser.Developer;

			if (isAdmin)
				userType |= TypeUser.Admin;

			return userType;
		}

		protected override void PreCycle()
		{
			_bot.SetWebhookAsync(string.Empty);
			_loger.WriteTrace($"{nameof(_bot.SetWebhookAsync)} successfully");
			 _offset = 0;
		}

		protected override void RunCycle()
		{
			var updates = _bot.GetUpdatesAsync(_offset, 1).Result;

			foreach (var update in updates)
			{
				if (update.Type == UpdateType.Message)
					EnqueueMessage(TelegramMessage(update.Message));

				if ((update.Id + 1) > _offset)
					_offset = update.Id + 1;

				_loger.WriteTrace(nameof(_offset) + update.Id);
			}
		}

		private TypeUser GetTypeUser(Telegram.Bot.Types.Message msg)
		{
			if (msg.Chat.Type == ChatType.Private)
				return GetTypeUser(true, msg);

			if (msg.Chat.AllMembersAreAdministrators)
				return GetTypeUser(true, msg);

			//backdoor, very bad!
			//if (msg.From.Id == myUserId)
			//	return true;

			//ToDo: DateTime.Now - _chatAdministations[msg.Chat.Id].timeStemp) > second
			if (!_chatAdministations.ContainsKey(msg.Chat.Id) || (DateTime.Now - _chatAdministations[msg.Chat.Id].timeStemp).Minutes > UPDATETIMESTam_Minutes)
			{
				var admins = _bot.GetChatAdministratorsAsync(msg.Chat.Id, cancellationToken: _cancellationToken).Result;

				var chatAdmins = new ChatAdministrations();
				chatAdmins.UserIds.AddRange(admins.Select(x => x.User.Id));
				chatAdmins.timeStemp = DateTime.Now;

				if (_chatAdministations.ContainsKey(msg.Chat.Id))
					_chatAdministations[msg.Chat.Id] = chatAdmins;
				else
					_chatAdministations.TryAdd(msg.Chat.Id, chatAdmins);
			}
			return GetTypeUser(_chatAdministations[msg.Chat.Id].UserIds.Contains(msg.From.Id), msg);
		}

		public override void Dispose()
		{
			//ToDo: _cancellationToken
			throw new NotImplementedException();
		}

		protected TelegramMessage TelegramMessage(Message msg) =>  new TelegramMessage(msg, GetTypeUser(msg));

		//ToDo
		private string GetCorrectWebText(string text) => text;

		public override async Task<IMessage> Message(CommandMessage message, Guid chatId)
		{
			long longChatId = chatId.ToLong();
			//ToDo: bug.
			if (longChatId == -429358480) longChatId = -1001156738448;

			var replaceMsg = message.OnIdMessage.ToInt();

			Message senderMsg = null;

			switch (message.TypeMessage)
			{
				case Types.Enums.MessageType.Text:
					var parseMode =  message.WithHtmlTags ? ParseMode.Html : ParseMode.Default;
					var text = message.WithHtmlTags ? GetCorrectWebText(message.Text) : message.Text;
					//text = _telegramHTML.RemoveAllTag(text);
					if (!string.IsNullOrEmpty(text))
						senderMsg = await _bot.SendTextMessageAsync(longChatId, text, parseMode, replyToMessageId: replaceMsg, disableWebPagePreview: true, cancellationToken: _cancellationToken); 
					break;
				case Types.Enums.MessageType.Coordinates:
					senderMsg = await _bot.SendLocationAsync(longChatId, message.Coord.Latitude, message.Coord.Longitude, replyToMessageId: replaceMsg, cancellationToken: _cancellationToken);
					//senderMsg = await _bot.Send
					break;
				case Types.Enums.MessageType.Photo:
					//var lstindex = message.FileToken.LastIndexOf("/");
					//var number = message.FileToken.Substring(lstindex + 1, 2);
					var parseModet = message.WithHtmlTags ? ParseMode.Html : ParseMode.Default;
					//todo: remove, test, linux
					if (message.FileToken.Type == Types.Enums.FileType.Local)
					{
						using (var stream = _fileWorker.ReadStream(message.FileToken))
							senderMsg = await _bot.SendPhotoAsync(longChatId, stream, message.Text, parseModet, replyToMessageId: replaceMsg, cancellationToken: _cancellationToken);
					}
					else
					{
						var photo = new InputOnlineFile(message.FileToken.Url);
						senderMsg = await _bot.SendPhotoAsync(longChatId, photo, message.Text, replyToMessageId: replaceMsg, cancellationToken: _cancellationToken);
					}
					break;
				case Types.Enums.MessageType.Document:
					using (var file = _fileWorker.ReadStream(message.FileToken))
						senderMsg = await _bot.SendDocumentAsync(longChatId, file, message.Text, replyToMessageId: replaceMsg, cancellationToken: _cancellationToken);
					break;
			}
			return senderMsg == null? null : TelegramMessage(senderMsg);
		}

		private async void DownloadFile(string fileId, IFileToken token, IMessage msg, TypeResource type)
		{
			try
			{
				var tFile = await _bot.GetFileAsync(fileId, cancellationToken: _cancellationToken);
				
				using (var file = _fileWorker.WriteStream(token))
				{
					var a = _bot.DownloadFileAsync(tFile.FilePath, file, cancellationToken: _cancellationToken);
					a.Wait();
				}

				msg.Resource = new Resource(token, type);
				_messagesQueue.Enqueue(msg);
			}
			catch (Exception ex)
			{
				_loger.WriteError("Error downloading: " + ex.Message);
			}
		}

		protected override void DownloadFile(IMessage msg)
		{
			DownloadFile(msg, msg);
		}

		protected void DownloadFile(IMessage msg, IMessage resourceMsg)
		{
			if (resourceMsg == null)
				return;

			switch (resourceMsg.TypeMessage)
			{
				case Types.Enums.MessageType.Photo:
					DownloadPhotoAsync(msg, resourceMsg);
					break;
				case Types.Enums.MessageType.Voice:
					DownloadVoiceAsync(msg, resourceMsg);
					break;
				default:
					DownloadFile(msg, msg.ReplyToMessage);
					break;
			}
		}

		protected void DownloadVoiceAsync(IMessage msg, IMessage resourceMsg)
		{
			var tMsg = (resourceMsg as TelegramMessage)?.Message;
			if (tMsg == null)
				return;

			//var filePath = SettingHelper.DontExistFile("ogg", resourceMsg.ChatId);
			var fileToken = _chatFileWorker(msg.ChatId).NewFileTokenByExt(".ogg");
			DownloadFile(tMsg.Voice.FileId, fileToken, msg, TypeResource.Voice);
		}

		protected void DownloadPhotoAsync(IMessage msg, IMessage resourceMsg)
		{
			var tMsg = (resourceMsg as TelegramMessage)?.Message;
			if (tMsg == null)
				return;

			//var filePath = SettingHelper.DontExistFile("jpg", resourceMsg.ChatId);
			var fileToken = _chatFileWorker(msg.ChatId).NewFileTokenByExt(".jpg");
			DownloadFile(tMsg.Photo[tMsg.Photo.Length - 1].FileId, fileToken, msg, TypeResource.Photo);
		}
	}
}