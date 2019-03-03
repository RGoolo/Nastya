using Model.Logic.Google;
using Model.Types.Attribute;
using Model.Types.Class;
using Model.Types.Enums;
using Model.Types.Interfaces;
using System;
using System.Threading.Tasks;

namespace Nastya.Commands
{

	[CommandClass("VisionMsg", "Распознаем текст", TypeUser.User)]
	public class RecognitionMsg : BaseCommand
	{
		public override event SendMsgDel SendMsg;

		public enum TypeEnum
		{
			Text, Web, Logo, Landmark, Doc,
		}

		[Command("CheckVision", "Расшифровывать текст с фото.")]
		public bool IsCheckVisionMsg { get; set; }

		[Command("IsCheckVoice", "Расшифровывать текст с голоса.")]
		public bool IsCheckVisionVoice { get; set; }

		[Command("text", "Получить текст с изображения.", resource: TypeResource.Photo)]
		public void GetText(IFileToken token) => ConvertToText(Vision.GetTextAsync, token);

		[Command("logo", "Получить список logo с изображения.", resource: TypeResource.Photo)]
		public void GetLogo(IFileToken token) => ConvertToText(Vision.GetLogoAsync, token);

		[Command("imgweb", "Получить список web с изображения.", resource: TypeResource.Photo)]
		public void GetWeb(IFileToken token) => ConvertToText(Vision.GetWebAsync, token);

		[Command("imgmark", "Получить список Landmark с изображения.", resource: TypeResource.Photo)]
		public void GetLandmark(IFileToken token) => ConvertToText(Vision.GetLandmarkAsync, token);

		[Command("imgdoc", "Получить список Landmark с изображения.", resource: TypeResource.Photo)]
		public void GetDoc(IFileToken token) => ConvertToText(Vision.GetDocAsync, token);

		[Command("text", "Получить текст из голосового файла.", resource: TypeResource.Voice)]
		public void GetVoiceText(IFileToken token) => ConvertToText(Voice.GetText, token);

		//private Coordinates _coord = new CheckCoordinates();
		[CommandOnMsg(nameof(IsCheckVisionMsg), MessageType.Photo, typeUser: TypeUser.User)]
		public void GetTextMsg(IMessage msg)
		{
			if (msg.Resource == null) return;
			GetText(msg.Resource.File);
		}
		
		//private Coordinates _coord = new CheckCoordinates();
		[CommandOnMsg(nameof(IsCheckVisionMsg), MessageType.Voice, typeUser: TypeUser.User)]
		public void GetTextMsgVoice(IMessage msg)
		{
			if (msg.Resource == null) return;
			GetVoiceText(msg.Resource.File);
		}

		private async void ConvertToText(Func<IChatFileWorker, IFileToken, Task<string>> toText, IFileToken token)
		{
			var text = await toText(FileWorker, token);

			var command = CommandMessage.GetTextMsg(text);
			command.WithHtmlTags = true;
			var transaction = new TransactionCommandMessage(command);
			SendMsg?.Invoke(transaction);
		}

	}
}
	
