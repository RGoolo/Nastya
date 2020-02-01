using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Telegram.Bot.Types.Enums;
using System.Net;
using Model.Logic.Settings;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using System.Threading;
using System.Security;
using Model.BotTypes.Class;
using Model.BotTypes.Enums;
using Model.BotTypes.Interfaces;
using Model.BotTypes.Interfaces.Messages;
using Model.Files.FileTokens;
using Model.Logger;
using MessageType = Model.BotTypes.Enums.MessageType;

namespace Model.TelegramBot
{
	public class TelegramBot : IConcurrentBot
	{
		private class Resource : IResource
		{
			public Resource(IChatFile file, TypeResource type)
			{
				File = file;
				Type = type;
			}

			public IChatFile File { get; }
			public TypeResource Type { get; }
		}

		private readonly SecureString _token;
		protected readonly ILogger _log;
		private readonly CancellationToken _cancellationToken = default(CancellationToken);
		private const int UpdatetimesTimeMinutes = 5;
		private const int MyUserId = 62779148;
		private const long MyChat = 62779148;

		private int _offset;
		private Dictionary<long, ChatAdministrations> _chatAdministations = new Dictionary<long, ChatAdministrations>();
		private TelegramHTML _telegramHtml = new TelegramHTML();
		private Telegram.Bot.TelegramBotClient _bot;

		private IChatFileFactory _chatFileWorker(IChatId chatId) => SettingsHelper.GetSetting(chatId).FileChatFactory;
		// private IChatFileWorker _fileWorker => SettingsHelper.FileWorker;

		public TypeBot TypeBot => TypeBot.Telegram;

		public ILogger Log => _log;

		public IBotId Id { get; }

		public TelegramBot(SecureString token, IBotId id)
		{
			Id = id;
			_log = Logger.Logger.CreateLogger(id.ToString());
			_log.Info(".ctor");
			var cred = new NetworkCredential(string.Empty, token).Password;
			_bot = new Telegram.Bot.TelegramBotClient(cred);//, WebProxyExtension.Create());
			//ToDo: SetWebHook 
			//ToDo: GetMeAsync
			_token = token;


			_bot.SetWebhookAsync(string.Empty);
			_log.Warning($"{nameof(_bot.SetWebhookAsync)} successfully");
			_offset = 0;
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

		public List<IBotMessage> GetMessages()
		{
			var updates = _bot.GetUpdatesAsync(_offset, 1).Result;
			var msgs = new List<IBotMessage>();

			foreach (var update in updates)
			{
				if (update.Type == UpdateType.Message)
					msgs.Add(TelegramMessage(update.Message));

				if ((update.Id + 1) > _offset)
					_offset = update.Id + 1;

				_log.Warning(nameof(_offset) + update.Id);
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
			if (!_chatAdministations.ContainsKey(msg.Chat.Id) || (DateTime.Now - _chatAdministations[msg.Chat.Id].timeStemp).Minutes > UpdatetimesTimeMinutes)
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

		public static (string text, ParseMode mode) GetText(Texter text)
		{
			try
			{
				var t = TelegramHTML.RemoveTag(text); //, GetParseMod(text));
				if (TelegramHTML.CheckPaarTags(t))
					return (t, GetParseMod(text));
			}
			catch (Exception e)
			{
				{ Logger.Logger.CreateLogger(nameof(CookieContainer)).Warning(e); }
			}
			return (TelegramHTML.RemoveAllTag(text?.Text), ParseMode.Default);
		}

		private static ParseMode GetParseMod(Texter text) => text?.Html == true ? ParseMode.Html : ParseMode.Default;

		public Task<IBotMessage> Message(IMessageToBot message, IChatId chatId)
		{
			try
			{
				return InternalMessage(message, chatId);
			}
			catch (Exception ex)
			{
				_log.Error(ex);
			}

			return null;
		}
		
		public async Task<IBotMessage> InternalMessage(IMessageToBot message, IChatId chatId)
		{

			_log.Info($"{nameof(message.TypeMessage)}:{message.TypeMessage} type:{message.Text?.Html} message:{ message.Text?.Text}");

			var longChatId = IdsMapper.ToLong(chatId.GetId); // ToDo is
			if (longChatId != 62779148)
				return null;

			var (text, mode) = GetText(message.Text);
			var replaceMsg = IdsMapper.ToInt(message.OnIdMessage?.GetId); // ToDo is
			var editMsg = IdsMapper.ToInt(message.EditMsg?.GetId); // ToDo is

			Message senderMsg = null;

			switch (message.TypeMessage)
			{
				case MessageType.Edit:
					if (!string.IsNullOrEmpty(text))
						senderMsg = await _bot.EditMessageTextAsync(longChatId, editMsg, text, mode,
							true, cancellationToken: _cancellationToken);
					break;

				case MessageType.Text:
					if (!string.IsNullOrEmpty(text))
						senderMsg = await _bot.SendTextMessageAsync(longChatId, text, mode,
							replyToMessageId: replaceMsg, disableWebPagePreview: true,
							cancellationToken: _cancellationToken);
					break;

				case MessageType.Coordinates:
					_log.Warning($"{message.Coordinate.Latitude}:{message.Coordinate.Longitude} replyToMessageId:{replaceMsg} longChatId:{longChatId}");

					senderMsg = await _bot.SendVenueAsync(longChatId, message.Coordinate.Latitude,
						message.Coordinate.Longitude, "title", "adress", replyToMessageId: replaceMsg,
						cancellationToken: _cancellationToken);
					break;

				case MessageType.Photo:
					senderMsg = await _bot.SendPhotoAsync(longChatId, message.FileToken.GetInputFile(), text, mode,
						replyToMessageId: replaceMsg, cancellationToken: _cancellationToken);
					break;

				case MessageType.Voice:
					senderMsg = await _bot.SendVoiceAsync(longChatId, message.FileToken.GetInputFile(), text, mode,
							replyToMessageId: replaceMsg, cancellationToken: _cancellationToken);
					break;

				case MessageType.Document:
					senderMsg = await _bot.SendDocumentAsync(longChatId, message.FileToken.GetInputFile(), text,
						mode, replyToMessageId: replaceMsg,
						cancellationToken: _cancellationToken);
					break;
				case MessageType.SystemMessage:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return senderMsg == null ? null : new NotificationMessage(TelegramMessage(senderMsg), message);
		}

		private async Task<IBotMessage> DownloadFileAsync(string fileId, IChatFile token, IBotMessage msg, TypeResource type)
		{
			try
			{
				var tFile = await _bot.GetFileAsync(fileId, _cancellationToken);

				await using (var file = token.WriteStream())
				{
					var a = _bot.DownloadFileAsync(tFile.FilePath, file, _cancellationToken);
					a.Wait(_cancellationToken);
				}

				msg.Resource = new Resource(token, type);
				return msg;
			}
			catch (Exception ex)
			{
				_log.Error(ex, "Error downloading: " );
			}
			return null;
		}

		public async Task<IBotMessage> DownloadFileAsync(IBotMessage msg) => await DownloadFileAsync(msg, msg);

		protected Task<IBotMessage> DownloadFileAsync(IBotMessage msg, IBotMessage resourceMsg)
		{
			if (resourceMsg == null)
				return null;

			return resourceMsg.TypeMessage switch
			{
				MessageType.Photo => DownloadPhotoAsync(msg, resourceMsg),
				MessageType.Voice => DownloadVoiceAsync(msg, resourceMsg),
				MessageType.Document => DownloadDocumentAsync(msg, msg.ReplyToMessage),
				_ => DownloadFileAsync(msg, resourceMsg.ReplyToMessage),
			};
		}

		protected Task<IBotMessage> DownloadVoiceAsync(IBotMessage msg, IBotMessage resourceMsg)
		{
			var tMsg = (resourceMsg as TelegramMessage)?.Message;
			if (tMsg == null)
				return null;

			//var filePath = SettingHelper.DontExistFile("ogg", resourceMsg.ChatId);
			var file = _chatFileWorker(msg.ChatId).NewResourcesFileByExt(".ogg");
			return DownloadFileAsync(tMsg.Voice.FileId, file, msg, TypeResource.Voice);
		}

		protected Task<IBotMessage> DownloadPhotoAsync(IBotMessage msg, IBotMessage resourceMsg)
		{
			var tMsg = (resourceMsg as TelegramMessage)?.Message;
			if (tMsg == null)
				return null;

			//var filePath = SettingHelper.DontExistFile("jpg", resourceMsg.ChatId);
			var fileToken = _chatFileWorker(msg.ChatId).NewResourcesFileByExt(".jpg");
			return DownloadFileAsync(tMsg.Photo[^1].FileId, fileToken, msg, TypeResource.Photo);
		}

		protected Task<IBotMessage> DownloadDocumentAsync(IBotMessage msg, IBotMessage resourceMsg)
		{
			var tMsg = (resourceMsg as TelegramMessage)?.Message;
			if (tMsg == null)
				return null;

			//var filePath = SettingHelper.DontExistFile("jpg", resourceMsg.ChatId);
			var fileToken = _chatFileWorker(msg.ChatId).NewResourcesFileByExt(System.IO.Path.GetExtension(tMsg.Document.FileName));
			return DownloadFileAsync(tMsg.Document.FileId, fileToken, msg, TypeResource.Document);
		}

		public void OnError(Exception ex)
		{
			/*//Временное, настроить VPN!;
			_log.Warning(nameof(OnError));
			var cred = new NetworkCredential(string.Empty, _token).Password;
			//_bot = new Telegram.Bot.TelegramBotClient(cred, WebProxyExtension.Create());

			_bot.SetWebhookAsync(string.Empty);
			_log.Warning($"{nameof(_bot.SetWebhookAsync)} successfully");*/
		}
	}
}