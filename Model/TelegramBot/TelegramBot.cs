using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Telegram.Bot.Types.Enums;
using System.Net;
using System.Runtime.CompilerServices;
using Model.Logic.Settings;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using System.Threading;
using System.Security;
using Model.BotTypes;
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
		protected readonly ILogger _log;
		private readonly CancellationToken _cancellationToken = default(CancellationToken);
		private const int UpdatetimesTimeMinutes = 5;
		private const int MyUserId = 62779148;
		private const long MyChat = 62779148;

		private int _offset;
		private Dictionary<long, ChatAdministrations> _chatAdministations = new Dictionary<long, ChatAdministrations>();
		private TelegramHtml _telegramHtml = new TelegramHtml();
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
					msgs.Add(TelegramMessage(update.Message, false));

				if ((update.Id + 1) > _offset)
					_offset = update.Id + 1;

				_log.Warning(nameof(_offset) + update.Id);
			}

			return msgs;
		}

		private TypeUser GetTypeUser(Message msg, bool isBot)
		{
			if (isBot)
				return TypeUser.Bot;
			
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

		protected TelegramMessage TelegramMessage(Message msg, bool isBot, IMessageToBot message = null) =>  new TelegramMessage(msg, GetTypeUser(msg, isBot), message);

		public static Texter GetNormalizeText(Texter text, IChatId chatId)
		{
			if (text?.Html != true)
				return text;

			try
			{
				if (text.ReplaceCoordinates)
				{
					var set = SettingsHelper.GetSetting(chatId);
					var points = set.PointsFactory.GetCoordinates(text.Text);
					text.Replace(points.ReplacePoints(), true);
				}

				

				var t = TelegramHtml.RemoveTag(text); //, GetParseMod(text));
				if (TelegramHtml.CheckPaarTags(t))
					return text.Replace(t, true);
			}
			catch (Exception e)
			{
				{ Logger.Logger.CreateLogger(nameof(CookieContainer)).Warning(e); }
			}
			return text.Replace(TelegramHtml.RemoveAllTag(text?.Text), false);
		}

		private static ParseMode GetParseMod(Texter text) => text?.Html == true ? ParseMode.Html : ParseMode.Default;

		//ToDo do it easy 
		public List<IMessageToBot> ChildrenMessage(IMessageToBot msg, IChatId chatId)
		{
			var result = new List<IMessageToBot>();
			if (msg.Text?.Html != true || (msg.TypeMessage & MessageType.Text) == 0 || string.IsNullOrEmpty(msg.Text.Text))
				return result;

			var setting = SettingsHelper.GetSetting(chatId);
			var defaultUrl = setting.Web.DefaultUri;

			var links = TelegramHtml.GetLinks(msg.Text.Text, defaultUrl);
			if (links.Count == 0 || !msg.Text.ReplaceResources)
				return result;

			var str = TelegramHtml.ReplaceTagsToHref(msg.Text.Text, links);

			msg.Text.Replace(str, true);
			var img = 0;
			foreach (var link in links)
			{
				switch (link.TypeUrl)
				{
					case TypeUrl.Img:
						if (img++ > msg.Text.Settings.MaxParsePicture)
							continue;

						var file = setting.TypeGame.IsDummy()
							? setting.FileChatFactory.GetExistFileByPath(link.Url)
							: setting.FileChatFactory.InternetFile(link.Url);
						result.Add(MessageToBot.GetPhototMsg(file, (Texter)link.Name));
						break;
					case TypeUrl.Sound:
						result.Add(MessageToBot.GetVoiceMsg(link.Url, link.Name));
						break;
				}
			}

			return result;
		}

		public async Task<IBotMessage> Message(IMessageToBot message, IChatId chatId)
		{
			try
			{
				return await InternalMessage(message, chatId); //ToDo delete try and cath on lvl up
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
			if (longChatId != MyChat)
				return null;

			var texter = GetNormalizeText(message.Text, chatId);

			var text = texter?.Text;
			var mode = GetParseMod(texter);

			var replaceMsg = IdsMapper.ToInt(message.OnIdMessage?.GetId); // ToDo is
			var editMsg = IdsMapper.ToInt(message.EditMsg?.GetId); // ToDo is

			Message senderMsg = null;

			switch (message.TypeMessage)
			{
				case MessageType.Edit:
					if (!string.IsNullOrWhiteSpace(text))
						senderMsg = await _bot.EditMessageTextAsync(longChatId, editMsg, text, mode,
							true, cancellationToken: _cancellationToken);
					break;

				case MessageType.Text:
					if (!string.IsNullOrWhiteSpace(text))
						senderMsg = await _bot.SendTextMessageAsync(longChatId, text, mode,
							replyToMessageId: replaceMsg, disableWebPagePreview: true,
							cancellationToken: _cancellationToken);
					break;

				case MessageType.Coordinates:
					_log.Warning($"{message.Coordinate.Latitude}:{message.Coordinate.Longitude} replyToMessageId:{replaceMsg} longChatId:{longChatId}");

					senderMsg = await _bot.SendVenueAsync(longChatId, message.Coordinate.Latitude,
						message.Coordinate.Longitude, message.Coordinate.Alias.ToString() ?? "", text, replyToMessageId: replaceMsg,
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

			return senderMsg == null ? null : TelegramMessage(senderMsg, true, message);
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