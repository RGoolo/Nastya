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

	[CommandClass("VisionMsg", "Распознаем текст с картинки", TypeUser.User)]
	public class RecognitionImegs
	{
		private readonly IChatService _settings;

		public RecognitionImegs(IChatService settings)
		{
			_settings = settings;
		}

		public enum TypeEnum
		{
			Text, Web, Logo, Landmark, Doc,
		}

		[Command("CheckVision", "Расшифровывать текст с фото.")]
		public Task<string> IsCheckVisionMsg { get; set; }

		[Command("text", "Получить текст с изображения.", resource: TypeResource.Photo)]
		public Task<string> GetText(IChatFile token, IChatFileFactory factory) => ConvertToText(Vision.GetTextAsync, token, factory);

		[Command("logo", "Получить список logo с изображения.", resource: TypeResource.Photo)]
		public Task<string> GetLogo(IChatFile token, IChatFileFactory factory) => ConvertToText(Vision.GetLogoAsync, token, factory);

		[Command("imgweb", "Получить список web с изображения.", resource: TypeResource.Photo)]
		public Task<string> GetWeb(IChatFile token, IChatFileFactory factory) => ConvertToText(Vision.GetWebAsync, token, factory);

		[Command("imgmark", "Получить список Landmark с изображения.", resource: TypeResource.Photo)]
		public Task<string> GetLandmark(IChatFile token, IChatFileFactory factory) => ConvertToText(Vision.GetLandmarkAsync, token, factory);

		[Command("imgdoc", "Получить список Landmark с изображения.", resource: TypeResource.Photo)]
		public Task<string> GetDoc(IChatFile token, IChatFileFactory factory) => ConvertToText(Vision.GetDocAsync, token, factory);

		[CommandOnMsg(nameof(GetTextMsg), MessageType.Photo, typeUser: TypeUser.User)]
		public async Task<string> GetTextMsg(IBotMessage msg, IChatFileFactory factory)
		{
			if (msg.Resource == null) return null;
            return await GetText(msg.Resource.File, factory);
		}

		private async Task<string> ConvertToText(Func<IChatFile, IChatFileFactory, Task<string>> toText, IChatFile token, IChatFileFactory factory)
		{
			return await toText(token, factory);
			//return MessageToBot.GetTextMsg(new Texter(text));
			//var transaction = new TransactionCommandMessage(command);
			//	SendMsg.Send(transaction);
		}
	}
}
	
