namespace Model.Bots.DiscordBot
{
/*
	public class DiscordBot : IConcurentBot
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
	
		private DiscordSocketClient _bot = new DiscordSocketClient(); 
		private IChatFileWorker _chatFileWorker(Guid chatId) => SettingsHelper<SettingHelper>.GetSetting(chatId).FileWorker;
		private IFileWorker _fileWorker => SettingsHelper<SettingHelper>.FileWorker;

		public TypeBot TypeBot => TypeBot.Telegram;

		public Logger Log => _log;

		public Guid Id { get; }

		public DiscordBot(SecureString token, Guid id)
		{
			Id = id;
			_log = new Logger(id.ToString());
			_log.WriteTrace(".ctor");
			var cred = new NetworkCredential(string.Empty, token).Password;
			var token1 = cred;


			_bot.LoginAsync(TokenType.Bot, token1);
			_bot.StartAsync();

			// Block this task until the program is closed.
			 Task.Delay(-1);

		}

	
		public void PreCycle()
		{
			//_bot.SetWebhookAsync(string.Empty);
			_log.WriteTrace($"{nameof(_bot)} successfully");
			_offset = 0;
		}

		public List<IMessage> RunCycle()
		{
			var updates = _bot.MessageUpdated += _bot_MessageUpdated; (_offset, 1).Result;
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

		private Task _bot_MessageUpdated(Cacheable<Discord.IMessage, ulong> arg1, SocketMessage arg2, ISocketMessageChannel arg3)
		{
			//arg1.Value.
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

		protected TelegramMessage TelegramMessage(Message msg) => new TelegramMessage(msg, GetTypeUser(msg));

		public static (string text, ParseMode mode) GetText(Texter text)
		{
			try
			{
				return (TelegramHTML.RemoteTag(text), GetParseMod(text));
			}
			catch (Exception e)
			{
				return (text?.Text, ParseMode.Default);
			}
		}

		private static ParseMode GetParseMod(Texter text) => text?.Html == true ? ParseMode.Html : ParseMode.Default;

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
						senderMsg = await _bot.SendTextMessageAsync(longChatId, text, mode,
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
						await using var stream = _fileWorker.ReadStream(message.FileToken);
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
	}*/
}