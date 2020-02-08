using System;
using System.Threading.Tasks;
using Model.BotTypes.Attribute;
using Model.BotTypes.Class;
using Model.BotTypes.Enums;
using Model.BotTypes.Interfaces.Messages;
using Model.Files.FileTokens;
using Model.Logic.Google;

namespace Nastya.Commands
{
	[CommandClass("SoundMsg", "Распознаем текст с войсом", TypeUser.User)]
	public class RecognitionSounds
	{
		[Command("IsCheckVoice", "Расшифровывать текст с голоса.")]
		public bool IsCheckVoice { get; set; }

		[Command("text", "Получить текст из голосового файла.", resource: TypeResource.Voice)]
		public Task<string> GetVoiceText(IChatFile token) => ConvertToText(Voice.GetText, token);

		//private Coordinates _coord = new CheckCoordinates();
		[CommandOnMsg(nameof(IsCheckVoice), MessageType.Photo, typeUser: TypeUser.User)]
		public void GetTextMsg(IBotMessage msg)
		{
			if (msg.Resource == null) return;
			GetVoiceText(msg.Resource.File);
		}

		//private Coordinates _coord = new CheckCoordinates();

		[CommandOnMsg(nameof(IsCheckVoice), MessageType.Voice, typeUser: TypeUser.User)]
		public void GetTextMsgVoice(IBotMessage msg)
		{
			if (msg.Resource == null || msg?.Resource?.Type != TypeResource.Voice) return;
			GetVoiceText(msg.Resource.File);
		}


		[CommandOnMsg(nameof(IsCheckVoice), MessageType.SystemMessage, typeUser: TypeUser.User)]
		public Task<string> Notifications(IBotMessage msg, IChatFileFactory factory)
		{
			if (msg.ReplyToCommandMessage?.FileToken == null)
				return null;

			// ToDo check voice!!!
			return GetVoiceText(factory.GetChatFile(msg.ReplyToCommandMessage.FileToken));
		}

		private async Task<string> ConvertToText(Func<IChatFile, Task<string>> toText, IChatFile token)
		{
			var text = await toText(token);
			return string.IsNullOrEmpty(text) ? "не удалось распознать сообщение" : text;
			//SendMsg.SendMsg(transaction);
		}
	}
}