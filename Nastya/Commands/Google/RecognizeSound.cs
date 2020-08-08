﻿using System;
using System.Threading.Tasks;
using Model.Bots.BotTypes.Attribute;
using Model.Bots.BotTypes.Enums;
using Model.Bots.BotTypes.Interfaces.Messages;
using Model.Files.FileTokens;
using Model.Logic.Google;
using Model.Logic.Settings;

namespace Nastya.Commands.Google
{
	[CommandClass("SoundMsg", "Распознаем текст с аудио", TypeUser.User)]
	public class RecognitionSounds
	{
		private readonly ISettings _settings;

		public RecognitionSounds(ISettings settings)
		{
			_settings = settings;
		}

		[Command("IsCheckVoice", "Расшифровывать текст с голоса.")]
		public bool IsCheckVoice { get; set; }

		[Command("text", "Получить текст из голосового файла.", resource: TypeResource.Voice)]
		public Task<string> GetVoiceText(IChatFile token, IChatFileFactory factory) => ConvertToText(Voice.GetText, factory, token);

		[CommandOnMsg(nameof(IsCheckVoice), MessageType.Photo, typeUser: TypeUser.User)]
		public void GetTextMsg(IBotMessage msg, IChatFileFactory factory)
		{
			if (msg.Resource == null) return;
			GetVoiceText(msg.Resource.File, factory);
		}

		[CommandOnMsg(nameof(IsCheckVoice), MessageType.Voice, typeUser: TypeUser.User)]
		public void GetTextMsgVoice(IBotMessage msg, IChatFileFactory factory)
		{
			if (msg.Resource == null || msg?.Resource?.Type != TypeResource.Voice) return;
			GetVoiceText(msg.Resource.File, factory);
		}

		[CommandOnMsg(nameof(IsCheckVoice), MessageType.SystemMessage, typeUser: TypeUser.User)]
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