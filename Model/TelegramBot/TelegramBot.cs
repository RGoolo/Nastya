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
using System.Text.RegularExpressions;

namespace Model.TelegramBot
{
	/*internal static class WebProxyExtension
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
	}*/

	public class TelegramBot : IConcurentBot
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
		protected readonly Logger _log;
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

		public TypeBot TypeBot => TypeBot.Telegram;

		public Logger Log => _log;

		public Guid Id { get; }

		public TelegramBot(SecureString token, Guid id)
		{
			Id = id;
			_log = new Logger(id.ToString());
			_log.WriteTrace(".ctor");
			var cred = new NetworkCredential(string.Empty, token).Password;
			_bot = new Telegram.Bot.TelegramBotClient(cred);//, WebProxyExtension.Create());
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

		public void PreCycle()
		{
			_bot.SetWebhookAsync(string.Empty);
			_log.WriteTrace($"{nameof(_bot.SetWebhookAsync)} successfully");
			 _offset = 0;
		}

		public List<IMessage> RunCycle()
		{
			var updates = _bot.GetUpdatesAsync(_offset, 1).Result;
			var msgs = new List<IMessage>();

			foreach (var update in updates)
			{
				if (update.Type == UpdateType.Message)
					msgs.Add(TelegramMessage(update.Message));

				if ((update.Id + 1) > _offset)
					_offset = update.Id + 1;

				_log.WriteTrace(nameof(_offset) + update.Id);
			}

			return msgs;
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
				var admins = _bot.GetChatAdministratorsAsync(msg.Chat.Id, _cancellationToken).Result;

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

		public void Dispose()
		{
			//ToDo: _cancellationToken
			//throw new NotImplementedException();
		}

		protected TelegramMessage TelegramMessage(Message msg) =>  new TelegramMessage(msg, GetTypeUser(msg));

		public static (string text, ParseMode mode) GetText(Texter text) => (TelegramHTML.RemoteTag(text), GetParseMod(text));

		private static ParseMode GetParseMod(Texter text) => text.Html ? ParseMode.Html : ParseMode.Default;

		public async Task<IMessage> Message(CommandMessage message, Guid chatId)
		{
			_log.WriteTrace(
				$"{nameof(message.TypeMessage)}:{message.TypeMessage} type:{message.Texter?.Html} message:{ message.Texter?.Text}");

			var longChatId = chatId.ToLong();
			if (longChatId != 62779148)
				return null;

			(var text, var mode) = GetText(message.Texter);
			var replaceMsg = message.OnIdMessage.ToInt();
			Message senderMsg = null;

			switch (message.TypeMessage)
			{
				case Types.Enums.MessageType.Edit:

					if (!string.IsNullOrEmpty(text))
						senderMsg = await _bot.EditMessageTextAsync(longChatId, replaceMsg, text, mode,
							true, cancellationToken: _cancellationToken);
					break;
				case Types.Enums.MessageType.Text:
					if (!string.IsNullOrEmpty(text))
						senderMsg = await _bot.SendTextMessageAsync(longChatId, text, GetParseMod(message.Texter),
							replyToMessageId: replaceMsg, disableWebPagePreview: true,
							cancellationToken: _cancellationToken);
					break;
				case Types.Enums.MessageType.Coordinates:
					_log.WriteTrace($"{message.Coordinate.Latitude}:{message.Coordinate.Longitude} replyToMessageId:{replaceMsg} longChatId:{longChatId}");

					senderMsg = await _bot.SendVenueAsync(longChatId, message.Coordinate.Latitude,
						message.Coordinate.Longitude, "title", "adress", replyToMessageId: replaceMsg,
						cancellationToken: _cancellationToken);
					break;
				case Types.Enums.MessageType.Photo:
					if (message.FileToken.Type == Types.Enums.FileType.Local)
					{
						using var stream = _fileWorker.ReadStream(message.FileToken);
						senderMsg = await _bot.SendPhotoAsync(longChatId, stream, text, mode,
							replyToMessageId: replaceMsg, cancellationToken: _cancellationToken);
					}
					else
					{
						var photo = new InputOnlineFile(message.FileToken.Url);
						senderMsg = await _bot.SendPhotoAsync(longChatId, photo, "", GetParseMod(message.Texter),
							replyToMessageId: replaceMsg, cancellationToken: _cancellationToken);
					}

					break;
				case Types.Enums.MessageType.Document:
					using (var file = _fileWorker.ReadStream(message.FileToken))
						senderMsg = await _bot.SendDocumentAsync(longChatId, file, text,
							mode, replyToMessageId: replaceMsg,
							cancellationToken: _cancellationToken);
					break;
			}

			return senderMsg == null ? null : new NotificationMessage(TelegramMessage(senderMsg), message);
		}

		private async Task<IMessage> DownloadFileAsync(string fileId, IFileToken token, IMessage msg, TypeResource type)
		{
			try
			{
				var tFile = await _bot.GetFileAsync(fileId, _cancellationToken);
				
				using (var file = _fileWorker.WriteStream(token))
				{
					var a = _bot.DownloadFileAsync(tFile.FilePath, file, _cancellationToken);
					a.Wait(_cancellationToken);
				}

				msg.Resource = new Resource(token, type);
				return msg;
			}
			catch (Exception ex)
			{
				_log.WriteError("Error downloading: " + ex.Message);
			}
			return null;
		}

		public async Task<IMessage> DownloadFileAsync(IMessage msg)
		{
			return await DownloadFileAsync(msg, msg);
		}

		protected Task<IMessage> DownloadFileAsync(IMessage msg, IMessage resourceMsg)
		{
			if (resourceMsg == null)
				return null;

			switch (resourceMsg.TypeMessage)
			{
				case Types.Enums.MessageType.Photo:
					return DownloadPhotoAsync(msg, resourceMsg);
				case Types.Enums.MessageType.Voice:
					return DownloadVoiceAsync(msg, resourceMsg);
				default:
					return DownloadFileAsync(msg, msg.ReplyToMessage);
				}
		}

		protected Task<IMessage> DownloadVoiceAsync(IMessage msg, IMessage resourceMsg)
		{
			var tMsg = (resourceMsg as TelegramMessage)?.Message;
			if (tMsg == null)
				return null;

			//var filePath = SettingHelper.DontExistFile("ogg", resourceMsg.ChatId);
			var fileToken = _chatFileWorker(msg.ChatId).NewFileTokenByExt(".ogg");
			return DownloadFileAsync(tMsg.Voice.FileId, fileToken, msg, TypeResource.Voice);
		}

		protected Task<IMessage> DownloadPhotoAsync(IMessage msg, IMessage resourceMsg)
		{
			var tMsg = (resourceMsg as TelegramMessage)?.Message;
			if (tMsg == null)
				return null;

			//var filePath = SettingHelper.DontExistFile("jpg", resourceMsg.ChatId);
			var fileToken = _chatFileWorker(msg.ChatId).NewFileTokenByExt(".jpg");
			return DownloadFileAsync(tMsg.Photo[tMsg.Photo.Length - 1].FileId, fileToken, msg, TypeResource.Photo);
		}

		public void OnError(Exception ex)
		{
			//Временное, настроить VPN!;
			_log.WriteTrace(nameof(OnError));
			var cred = new NetworkCredential(string.Empty, _token).Password;
			//_bot = new Telegram.Bot.TelegramBotClient(cred, WebProxyExtension.Create());

			_bot.SetWebhookAsync(string.Empty);
			_log.WriteTrace($"{nameof(_bot.SetWebhookAsync)} successfully");
		}
	}
}