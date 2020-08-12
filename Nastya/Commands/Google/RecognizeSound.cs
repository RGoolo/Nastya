using System;
using System.Threading.Tasks;
using BotModel.Bots.BotTypes.Attribute;
using BotModel.Bots.BotTypes.Enums;
using BotModel.Bots.BotTypes.Interfaces.Messages;
using BotModel.Files.FileTokens;
using Model.Logic.Google;
using Model.Settings;


namespace NightGameBot.Commands.Google
{
	[CommandClass("SoundMsg", "Распознаем текст с аудио", TypeUser.User)]
	public class RecognitionSounds
	{
		private readonly IChatService _settings;

		public RecognitionSounds(IChatService settings)
		{
			_settings = settings;
		}

		[Command("IsCheckVoice", "Расшифровывать текст с голоса.")]
		public bool IsCheckVoice { get; set; }

		[Command("text", "Получить текст из голосового файла.", resource: TypeResource.Voice)]
		public Task<string> GetVoiceText(IChatFile token, IChatFileFactory factory) => ConvertToText(Voice.GetText, factory, token);

		[CommandOnMsg(nameof(IsCheckVoice), MessageType.Voice, typeUser: TypeUser.User | TypeUser.Bot)]
		public async Task<string> GetTextMsgVoice(IBotMessage msg, IChatFileFactory factory)
		{
			if (msg.Resource == null || msg?.Resource?.Type != TypeResource.Voice) return null;
			return await GetVoiceText(msg.Resource.File, factory);
		}

		[CommandOnMsg(nameof(IsCheckVoice), MessageType.SystemMessage, typeUser: TypeUser.User | TypeUser.Bot)]
		public Task<string> Notifications(IBotMessage msg, IChatFileFactory factory)
		{
			if (msg.ReplyToCommandMessage?.FileToken == null)
				return null;

			// ToDo check voice!!!
			return GetVoiceText(factory.GetChatFile(msg.ReplyToCommandMessage.FileToken), factory);
		}

		private async Task<string> ConvertToText(Func<IChatFile, IChatFile, Task<string>> toText,
            IChatFileFactory factory, IChatFile token)
		{
			var text = await toText(token, factory.SystemFile(SystemChatFile.RecognizeCredentials));
			return string.IsNullOrEmpty(text) ? "не удалось распознать сообщение" : text;
		}
	}
}