using System;
using System.Collections.Generic;
using System.Net;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using Model.Bots.BotTypes.Class;
using Model.Bots.BotTypes.Enums;
using Model.Bots.BotTypes.Interfaces;
using Model.Bots.BotTypes.Interfaces.Ids;
using Model.Bots.BotTypes.Interfaces.Messages;
using Model.Bots.TelegramBot.Services;
using Model.Logger;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using MessageType = Model.Bots.BotTypes.Enums.MessageType;

namespace Model.Bots.TelegramBot.Entity
{
	public class TelegramBot : IConcurrentBot<TelegramMessage>
	{
		private readonly ILogger _log;
		private readonly CancellationToken _cancellationToken = default(CancellationToken);
		
		private int _offset;

		private readonly Telegram.Bot.TelegramBotClient _bot;
		private readonly ChatMembersService _membersService;
		private readonly Downloader _downloader;

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

			_downloader = new Downloader(_bot, _cancellationToken);
			_membersService = new ChatMembersService(_bot, _cancellationToken);

			_bot.SetWebhookAsync(string.Empty);
			_log.Info($"{nameof(_bot.SetWebhookAsync)} successfully");
			_offset = 0;
		}

		public List<TelegramMessage> GetMessages()
		{
			var updates = _bot.GetUpdatesAsync(_offset, 1).Result;
			var msgs = new List<TelegramMessage>();

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

		private TypeUser GetTypeUser(Message msg, bool isBot) => _membersService.GetTypeUser(msg, isBot);

		public void Dispose()
		{
			//ToDo: _cancellationToken
			//throw new NotImplementedException();
		}

		private TelegramMessage TelegramMessage(Message msg, bool isBot, IMessageToBot message = null) =>  new TelegramMessage(msg, GetTypeUser(msg, isBot), message);
		private static Texter GetNormalizeText(Texter text, IChatId chatId) => TexterService.GetNormalizeText(text, chatId);
		private static ParseMode GetParseMod(Texter text) => text?.Html == true ? ParseMode.Html : ParseMode.Default;
        private List<IMessageToBot> ChildrenMessage(TelegramMessage telegramMessage, IMessageToBot msg, IChatId chatId) => MessageService.ChildrenMessage(telegramMessage, msg, chatId);

		public async Task<TelegramMessage> SendMessage(IMessageToBot message, IChatId chatId)
		{
			try
			{
                var msg = await InternalMessage(message, chatId); //ToDo delete try and cath on lvl up

                var cms = ChildrenMessage(msg, message, chatId);

				foreach (var cm in cms)
                {
                    await SendMessage(cm, chatId);
                }

				return msg;
            }
			catch (Exception ex)
			{
				_log.Error(ex);
			}

			return null;
		}

		private async Task<TelegramMessage> InternalMessage(IMessageToBot message, IChatId chatId)
		{
			_log.Info($"{nameof(message.TypeMessage)}:{message.TypeMessage} type:{message.Text?.Html} message:{ message.Text?.Text}");

			var longChatId = IdsMapper.ToLong(chatId.GetId); // ToDo is
			//if (longChatId != MyChat)
			//	return null;

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

		public async Task<TelegramMessage> DownloadFileAsync(IBotMessage msg) => await _downloader.DownloadFileAsync(msg);

		
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