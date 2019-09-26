using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using Model.Logic.Coordinates;
using Model.Logic.Settings;
using Model.Types.Attribute;
using Model.Types.Class;
using Model.Types.Enums;
using Model.Types.Interfaces;

namespace Nastya.Commands
{

	//Display(Description = "Работа с сообщениями", Name = "coords"),

	[CommandClass("coords", "Работа с координатами.", TypeUser.User)]
	public class CheckCoordinates : BaseCommand
	{
		public CheckCoordinates()
		{
			//ToDo chat key
			var cred = new NetworkCredential(string.Empty, SecurityEnvironment.GetTextPassword("google_maps_token")).Password;
			_settings = new SettingsPoints();
			_pointsFactory = new PointsFactory(_settings, cred, FileWorker);
		}

		private SettingsPoints _settings;
		private PointsFactory _pointsFactory;
		private string _googleCreds;

		[Command(nameof(GoogleCreds), "Имя ссылки на Yandex карты.")]
		public string GoogleCreds
		{
			get => _googleCreds;
			set
			{
				_googleCreds = value;
				_pointsFactory = new PointsFactory(_settings, _googleCreds, FileWorker);
			}
		}

		[Command(nameof(NameYLink), "Имя ссылки на Yandex карты.")]
		public string NameYLink {
			get => _settings.Yandex.Name;
			set => _settings.Yandex.Name = value; }

		[Command(nameof(NameGLink), "Имя ссылки на Google карты.")]
		public string NameGLink {
			get => _settings.Google.Name;
			set => _settings.Google.Name = value; }

		[Command(nameof(NameYPoints), "Имя ссылки на Yandex маршрут от меня.")]
		public string NameYPoints {
			get => _settings.Yandex.PointName;
			set => _settings.Yandex.PointName = value; }

		[Command(nameof(NameGPoints), "Имя ссылки на Google маршрут от меня.")]
		public string NameGPoints {
			get => _settings.Google.PointName;
			set => _settings.Google.PointName = value; }

		[Command(nameof(NameYPointsMe), "Имя ссылки на Yandex маршрут.")]
		public string NameYPointsMe {
			get => _settings.Yandex.PointNameMe;
			set => _settings.Yandex.PointNameMe = value; }

		[Command(nameof(NameGPointsMe), "Имя ссылки на Google точки.")]
		public string NameGPointsMe {
			get => _settings.Google.PointNameMe;
			set => _settings.Google.PointNameMe = value; }

		[Command(nameof(CheckCoordintate), "Проверять сообщения на координаты.")]
		public bool CheckCoordintate { get; set; }

		[Command(nameof(AddYandex), "Добавлять в координаты ссылку на yandex map.")]
		public bool AddYandex {
			get => _settings.Yandex.LinkFor;
			set => _settings.Yandex.LinkFor = value; }

		[Command(nameof(GoogleToken), "Добавлять в координаты ссылку на yandex map.")]
		public string GoogleToken {
			get => string.Empty; //remove!!!
			set => SecurityEnvironment.SetPassword(value, "google_maps_token"); }

		/*	[Command(nameof(Dontaddkml), "Не добавляет kml файл к сообщениям на координаты.", "{F8F5407E-64A7-483A-82C6-FA26740ABB48}")]
		public bool Dontaddkml{ get; set; }*/

		[Command(nameof(AddGoogle), "Добавлять в координаты ссылку на google map.")]
		public bool AddGoogle {
			get => _settings.Google.LinkFor;
			set => _settings.Google.LinkFor = value; }

		[Command(nameof(AddPicture), "Добавлять картинку координат, при построении маршрутов")]
		public bool AddPicture
		{
			get => _settings.AddPicture;
			set => _settings.AddPicture = value;
		}

		[Command(Const.Game.City, "Установить город, для карт.")]
		public string City {
			get => _settings.City.ToString();
			set => _settings.City = _pointsFactory.GetCoordinate(value); }

		[CommandOnMsg(nameof(CheckCoordintate), MessageType.Text, typeUser: TypeUser.User)]
		public TransactionCommandMessage GetAllCoordinates(IMessage msg)
		{
			var result = new List<CommandMessage>();
			return new TransactionCommandMessage(_pointsFactory.GetCoordinates(msg.Text).Points().Select(CommandMessageWithDescription).ToList());
		}

		private CommandMessage CommandMessageWithDescription(Coordinate coord) =>
			CommandMessage.GetCoordMsg(coord, coord.OriginText);// _pointsFactory.GetUrlLink(coord, true));
		
		[Command(nameof(Coords), "Распознать координаты из сообщения.")]
		public TransactionCommandMessage Coords(IMessage msg) => 
			GetCoordinate(msg, nameof(Coords), (x) => _pointsFactory.GetCoordinates(x).Points());

		//[Command(nameof(Places), "Скинуть  из текста.")]
//		public TransactionCommandMessage Places(IMessage msg) =>
//			GetCoordinate(msg, null, _pointsFactory.GetPlaces);

		[Command(nameof(Route), "Построить маршрут по координатам из текста.")]
		public CommandMessage Route(IMessage msg) =>  
			GetCommand(msg, nameof(Route), _pointsFactory.GetCoordinates, !AddPicture);

		[Command(nameof(Places), "Построить маршрут, по местам, разделлеными новой строкой или ';'")]
		public CommandMessage Places(IMessage msg) =>
			GetCommand(msg, nameof(Places), (x) => _pointsFactory.GetPlaces(x), AddPicture);

	
		[Command(nameof(AddTextCoord), "Добавить построчно ссылки.")]
		public CommandMessage AddTextCoord(IMessage msg) => 
			GetCommand(msg,  "/" + nameof(AddTextCoord), _pointsFactory.GetCoordinates);

		private string GetText(IMessage msg, string fName)
		{
			return (msg.ReplyToMessage != null) 
				? msg.ReplyToMessage.Text 
				: string.IsNullOrEmpty(fName) ? msg.Text : msg.Text.Replace("/" + fName, "", true, CultureInfo.InvariantCulture);
		}
		
		private CommandMessage GetCommand<T>(IMessage msg, string fName, Func<string, PointWorker<T>> func, bool addPicture = false) where T: Point
		{
			var text = GetText(msg, fName);
			Console.WriteLine("text:" + text);
			CommandMessage result;
			var pointWorker = func(text);
			//if (isText.HasValue)
			{
				//var file = SettingsHelper.GetSetting(ChatId).FileWorker.NewFileTokenByExt(".jpg");

				var file = new LocalFileWorker(ChatId).NewFileTokenByExt(".jpg");
				//if (addPicture) _pointsFactory.GetPictureText(text, file);
				_pointsFactory.SetPicture(file, pointWorker.Points());

				result = CommandMessage.GetHTMLPhototMsg(file, pointWorker.TotalPoints());
			}
			//else
			{
				//result = CommandMessage.GetTextMsg(new Texter(func(text), true));
			}
			result.OnIdMessage = msg.MessageId;
			return result;
		}

		private TransactionCommandMessage GetCoordinate(IMessage msg, string fName, Func<string, IEnumerable<Coordinate>> func)
		{
			var text = GetText(msg, fName);
			var result = new List<CommandMessage>();

			var coords = func(text);

			coords.ToList().ForEach(x =>
			{
				result.Add(CommandMessage.GetTextMsg(x.OriginText + Environment.NewLine)); //+ _pointsFactory.GetUrlLink(x, true)));
				result.Add(CommandMessage.GetCoordMsg(x, x.OriginText));
			});
			return new TransactionCommandMessage(result) ;
		}

	}
}
