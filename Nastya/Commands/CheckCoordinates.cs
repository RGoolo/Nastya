using System;
using System.Collections.Generic;
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
			var cred = new NetworkCredential(string.Empty, SecurityEnvironment.GetPassword("google_map")).Password;
            _settings = new SettingsPoints();
            _coordinates = new PointsFactory(_settings, cred, FileWorker);
		}

        private SettingsPoints _settings;
        private PointsFactory _coordinates;
		
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
			set => _settings.City = _coordinates.GetCoordinate(value); }

		//private Coordinates _coord = new CheckCoordinates();
		/*[CommandOnMsg(nameof(CheckCoordintate), MessageType.Text, typeUser: TypeUser.User)]
		public TransactionCommandMessage GettAllCoordinates(IMessage msg)
		{
			var result = new List<CommandMessage>();
			return new TransactionCommandMessage(CoordinatesFactory.GetCoords(msg.Text).Select(CommandMessageWithDescription).ToList());
		}

		private CommandMessage CommandMessageWithDescription(Coordinate coord) => 
			CommandMessage.GetCoordMsg(coord, _coordinates.GetUrlLink(coord, true));
		
		[Command(nameof(Coords), "Скинуть в чат координаты из координат сообщения.")]
		public TransactionCommandMessage Coords(IMessage msg) => 
			GetCoordinate(msg, nameof(Coords), CoordinatesFactory.GetCoords);

		[Command(nameof(CoordsT), "Скинуть в чат координаты из текста.")]
		public TransactionCommandMessage CoordsT(IMessage msg) =>
			GetCoordinate(msg, null, _coordinates.GetTextCoord);

		[Command(nameof(Route), "Построить маршрут по координатам из текста.")]
		public CommandMessage Route(IMessage msg) =>  
			GetCommand(msg, null, _coordinates.GetPointes, !AddPicture);

		[Command(nameof(CreateMap), "Построить маршрут, где каждая строка пункт назначения.")]
		public CommandMessage CreateMap(IMessage msg) =>
			GetCommand(msg, null , _coordinates.GetTextPointes, AddPicture);

		[Command(nameof(AddCoord), "Добавить в сообщения ссылки на координаты.")]
		public CommandMessage AddCoord(IMessage msg) =>
			GetCommand(msg, nameof(AddCoord), _coordinates.ReplaceCoords, null);

		[Command(nameof(AddTextCoord), "Добавить построчно ссылки.")]
		public CommandMessage AddTextCoord(IMessage msg) => 
			GetCommand(msg, nameof(AddTextCoord), _coordinates.ReplaceTextCoords, null);

		private string GetText(IMessage msg, string fName)
		{
			return (msg.ReplyToMessage != null) 
				? msg.ReplyToMessage.Text 
				: string.IsNullOrEmpty(fName) ? msg.Text : msg.Text.Replace(fName, "", StringComparison.CurrentCultureIgnoreCase);
		}

		private CommandMessage GetCommand(IMessage msg, string fName, Func<string, string> func, bool? isText)
		{
			var text = GetText(msg, fName);
			CommandMessage result = null;
			if (isText.HasValue)
			{
				var file = SettingsHelper.GetSetting(ChatId).FileWorker.NewFileTokenByExt(".jpg");
				if (isText.Value) _coordinates.GetPictureText(text, file);
				else _coordinates.GetPicture(text, file);

				result = CommandMessage.GetPhototMsg(file, func(text));
			}
			else
			{
                result = CommandMessage.GetTextMsg(new Texter(func(text), true));
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
				result.Add(CommandMessage.GetTextMsg(x.OriginText + Environment.NewLine + _coordinates.GetUrlLink(x, true)));
				result.Add(CommandMessage.GetCoordMsg(x));
			});
			return new  TransactionCommandMessage(result);
		}*/

	}
}
