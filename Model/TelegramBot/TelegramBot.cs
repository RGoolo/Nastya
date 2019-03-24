using System;
using System.Collections.Generic;
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

		public static Random random = new Random(DateTime.Now.Millisecond);


		public static WebProxy Create()
		{
			var ran = random.Next(Proxies.Count - 1);
			return new WebProxy(Proxies[ran]);
		}
		
		static List<string> Proxies = new List<string>()
		{
			"62.149.12.98:3128",
			"51.75.33.220:3128",
			"134.209.230.82:8080",
			"46.4.115.48:3128",
			"91.211.247.26:8080",
			"145.239.92.81:3128",
			"91.211.247.26:80",
			"195.201.129.206:3128",
			"195.230.131.210:3128",
			"54.37.136.149:3128",
			"159.65.204.30:8080",
			"51.75.75.193:3128",
			"212.182.25.89:3128",
		};
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

		private TypeUser GetTypeUser(bool isAdmin, Message msg)
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

		private TypeUser GetTypeUser(Message msg)
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

			var replaceMsg = message.OnIdMessage.ToInt();

			Message senderMsg = null;

			switch (message.TypeMessage)
			{
				case Types.Enums.MessageType.Text:
					var parseMode =  message.WithHtmlTags ? ParseMode.Html : ParseMode.Default;
					var text = message.WithHtmlTags ? GetCorrectWebText(message.Text) : message.Text;
					//text = _telegramHtml.RemoveAllTag(text);
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
						senderMsg = await _bot.SendPhotoAsync(longChatId, photo, message.Text, parseModet, replyToMessageId: replaceMsg, cancellationToken: _cancellationToken);
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

		protected override void OnError(Exception ex)
		{
			//Временное, настроить VPN!;
			_loger.WriteTrace(nameof(OnError));
			var cred = new NetworkCredential(string.Empty, _token).Password;
			_bot = new Telegram.Bot.TelegramBotClient(cred, WebProxyExtension.Create());

			_bot.SetWebhookAsync(string.Empty);
			_loger.WriteTrace($"{nameof(_bot.SetWebhookAsync)} successfully");
		}
	}
}