using Model.Logic.Google;
using System;
using System.Threading.Tasks;
using Model.BotTypes.Attribute;
using Model.BotTypes.Class;
using Model.BotTypes.Enums;
using Model.BotTypes.Interfaces;
using Model.BotTypes.Interfaces.Messages;
using Model.Files.FileTokens;

namespace Nastya.Commands
{

	[CommandClass("VisionMsg", "Распознаем текст с картинки", TypeUser.User)]
	public class RecognitionImegs : BaseCommand
	{
		public enum TypeEnum
		{
			Text, Web, Logo, Landmark, Doc,
		}

		[Command("CheckVision", "Расшифровывать текст с фото.")]
		public Task<string> IsCheckVisionMsg { get; set; }

		[Command("text", "Получить текст с изображения.", resource: TypeResource.Photo)]
		public Task<string> GetText(IChatFile token) => ConvertToText(Vision.GetTextAsync, token);

		[Command("logo", "Получить список logo с изображения.", resource: TypeResource.Photo)]
		public Task<string> GetLogo(IChatFile token) => ConvertToText(Vision.GetLogoAsync, token);

		[Command("imgweb", "Получить список web с изображения.", resource: TypeResource.Photo)]
		public Task<string> GetWeb(IChatFile token) => ConvertToText(Vision.GetWebAsync, token);

		[Command("imgmark", "Получить список Landmark с изображения.", resource: TypeResource.Photo)]
		public Task<string> GetLandmark(IChatFile token) => ConvertToText(Vision.GetLandmarkAsync, token);

		[Command("imgdoc", "Получить список Landmark с изображения.", resource: TypeResource.Photo)]
		public Task<string> GetDoc(IChatFile token) => ConvertToText(Vision.GetDocAsync, token);

		//private Coordinates _coord = new CheckCoordinates();// ToDo to voice
		[CommandOnMsg(nameof(IsCheckVisionMsg), MessageType.Voice, typeUser: TypeUser.User)]
		public void GetTextMsgVoice(IBotMessage msg)
		{
			if (msg.Resource == null) return;
			GetText(msg.Resource.File);
		}

		private async Task<string> ConvertToText(Func<IChatFile, Task<string>> toText, IChatFile token)
		{
			return await toText(token);
			//return MessageToBot.GetTextMsg(new Texter(text));
			//var transaction = new TransactionCommandMessage(command);
			//	SendMsg.Send(transaction);
		}
	}
}
	
