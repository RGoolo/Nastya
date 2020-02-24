using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Model.BotTypes.Attribute;
using Model.BotTypes.Class;
using Model.BotTypes.Enums;
using Model.BotTypes.Interfaces;
using Model.BotTypes.Interfaces.Messages;
using Model.Files.FileTokens;
using Model.Logic.Coordinates;
using Model.Logic.Settings;
using Model.TelegramBot;

namespace Nastya.Commands
{

	//Display(Description = "Работа с сообщениями", Name = "coords"),

	[CommandClass("coords", "Работа с координатами.", TypeUser.User)]
	public class CoordinatesCommand
	{
		private ISettings Settings { get; }

		public CoordinatesCommand(ISettings settings)
		{
			Settings = settings;

			_pointsFactory = settings.PointsFactory;
		}

		private IPointsFactory _pointsFactory;
		private string _googleCreds;

		[Password]
		[Command(Const.Coordinates.Google.GoogleCreads, "creads")]
		public string GoogleCreds { get; set; }


		[Command(nameof(CheckCoordintate), "Проверять сообщения на координаты.")]
		public bool CheckCoordintate { get; set; }

		[Command(nameof(AddPicture), "Добавлять картинку координат, при построении маршрутов")]
		public bool AddPicture { get; set; }

		[CommandOnMsg(nameof(CheckCoordintate), MessageType.Text, typeUser: TypeUser.User)]
		public TransactionCommandMessage GetAllCoordinates(IBotMessage msg)
		{
			var result = new List<IMessageToBot>();
			return new TransactionCommandMessage(_pointsFactory.GetCoordinates(msg.Text).Points().Select(CommandMessageWithDescription).ToList());
		}

		[CommandOnMsg(nameof(CheckCoordintate), MessageType.Text, typeUser: TypeUser.Bot)]
		public async Task<TransactionCommandMessage> GetAllCoordinatesForBot(IBotMessage msg)
		{
			var result = new List<IMessageToBot>();
			if (string.IsNullOrEmpty(msg.Text))
				return null;

			var text = TelegramHtml.RemoveAllTag(msg.Text);
			var points = _pointsFactory.GetCoordinates(text).Points();
			if (points.Count == 0)
				return null;

			result.AddRange(points.Select(CommandMessageWithDescription));
		
			if (AddPicture && points.Count > 1)
			{
				var file = Settings.FileChatFactory.NewResourcesFileByExt(".jpg");
				await _pointsFactory.SetPicture(file, points);
				
				var textImg = string.Join("\n", points.Select(p => $"{p.Alias}) {p}"));
				result.Add(MessageToBot.GetPhototMsg(file, (Texter)textImg));
			}
			
			return new TransactionCommandMessage(result);
		}

		private IMessageToBot CommandMessageWithDescription(Coordinate coord) =>
			MessageToBot.GetCoordMsg(coord, coord.OriginText);// _pointsFactory.GetUrlLink(coord, true));
		
		[Command(nameof(Coords), "Распознать координаты из сообщения.")]
		public TransactionCommandMessage Coords(IBotMessage msg) => 
			GetCoordinate(msg, nameof(Coords), (x) => _pointsFactory.GetCoordinates(x).Points());

		//[Command(nameof(Places), "Скинуть  из текста.")]
//		public TransactionCommandMessage Places(IMessage msg) =>
//			GetCoordinate(msg, null, _pointsFactory.GetPlaces);

		[Command(nameof(Route), "Построить маршрут по координатам из текста.")]
		public Task<IMessageToBot> Route(IBotMessage msg, IChatFileFactory fWorker) =>  
			GetCommand(msg, nameof(Route), _pointsFactory.GetCoordinates, fWorker, !AddPicture);

		[Command(nameof(Places), "Построить маршрут, по местам, разделлеными новой строкой или ';'")]
		public Task<IMessageToBot> Places(IBotMessage msg, IChatFileFactory fWorker) =>
			GetCommand(msg, nameof(Places), (x) => _pointsFactory.GetPlaces(x), fWorker, AddPicture);

	
		[Command(nameof(AddTextCoord), "Добавить построчно ссылки.")]
		public Task<IMessageToBot> AddTextCoord(IBotMessage msg, IChatFileFactory fWorker) => 
			GetCommand(msg,  "/" + nameof(AddTextCoord), _pointsFactory.GetCoordinates, fWorker);

		private string GetText(IBotMessage msg, string fName)
		{
			return (msg.ReplyToMessage != null) 
				? msg.ReplyToMessage.Text 
				: string.IsNullOrEmpty(fName) ? msg.Text : msg.Text.Replace("/" + fName, "", true, CultureInfo.InvariantCulture);
		}
		
		private async Task<IMessageToBot> GetCommand<T>(IBotMessage msg, string fName, Func<string, PointWorker<T>> func, IChatFileFactory fWorker, bool addPicture = false) where T: Point
		{
			var text = GetText(msg, fName);
			Console.WriteLine("text:" + text);
			IMessageToBot result;
			var pointWorker = func(text);
			//if (isText.HasValue)
			{
				//var file = SettingsHelper.GetSetting(ChatId).FileWorker.NewFileTokenByExt(".jpg");

				var file = fWorker.NewResourcesFileByExt(".jpg");
				//if (addPicture) _pointsFactory.GetPictureText(text, file);
				await _pointsFactory.SetPicture(file, pointWorker.Points());

				result = MessageToBot.GetHTMLPhototMsg(file, pointWorker.TotalPoints());
			}
			//else
			{
				//result = CommandMessage.GetTextMsg(new Texter(func(text), true));
			}
			result.OnIdMessage = msg.MessageId;
			return result;
		}

		private TransactionCommandMessage GetCoordinate(IBotMessage msg, string fName, Func<string, IEnumerable<Coordinate>> func)
		{
			var text = GetText(msg, fName);
			var result = new List<IMessageToBot>();

			var coords = func(text);

			coords.ToList().ForEach(x =>
			{
				result.Add(MessageToBot.GetTextMsg((Texter)(x.OriginText + Environment.NewLine))); //+ _pointsFactory.GetUrlLink(x, true)));
				result.Add(MessageToBot.GetCoordMsg(x, x.OriginText));
			});
			return new TransactionCommandMessage(result) ;
		}

	}
}
