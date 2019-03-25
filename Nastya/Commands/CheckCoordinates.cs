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
			_coordinates = new Coordinates(FileWorker, cred);
		}

		private Coordinates _coordinates;
		
		[Command(nameof(NameYLink), "Имя ссылки на Yandex карты.")]
		public string NameYLink {
			get => _coordinates.YandexName;
			set => _coordinates.YandexName = value; }

		[Command(nameof(NameGLink), "Имя ссылки на Google карты.")]
		public string NameGLink {
			get => _coordinates.GoogleName;
			set => _coordinates.GoogleName = value; }

		[Command(nameof(NameYPoints), "Имя ссылки на Yandex маршрут от меня.")]
		public string NameYPoints {
			get => _coordinates.YandexPointNameMe;
			set => _coordinates.YandexPointNameMe = value; }

		[Command(nameof(NameGPoints), "Имя ссылки на Google маршрут от меня.")]
		public string NameGPoints {
			get => _coordinates.GooglePointNameMe;
			set => _coordinates.GooglePointNameMe = value; }

		[Command(nameof(NameYPointsMe), "Имя ссылки на Yandex маршрут.")]
		public string NameYPointsMe {
			get => _coordinates.YandexPointNameMe;
			set => _coordinates.YandexPointNameMe = value; }

		[Command(nameof(NameGPointsMe), "Имя ссылки на Google точки.")]
		public string NameGPointsMe {
			get => _coordinates.GooglePointNameMe;
			set => _coordinates.GooglePointNameMe = value; }

		[Command(nameof(CheckCoord), "Проверять сообщения на координаты.")]
		public bool CheckCoord { get; set; }

		[Command(nameof(AddYandex), "Добавлять в координаты ссылку на yandex map.")]
		public bool AddYandex { get; set; }

		/*	[Command(nameof(Dontaddkml), "Не добавляет kml файл к сообщениям на координаты.", "{F8F5407E-64A7-483A-82C6-FA26740ABB48}")]
		public bool Dontaddkml{ get; set; }*/

		[Command(nameof(AddGoogle), "Добавлять в координаты ссылку на google map.")]
		public bool AddGoogle { get; set; }

		[Command(nameof(AddPicture), "Добавлять картинку координат, при построении маршрутов")]
		public bool AddPicture { get; set; } = true;

		[Command(Const.Game.City, "Установить город, для карт.")]
		public string City {
			get => _coordinates.City.ToString();
			set => _coordinates.City = _coordinates.GetCoord(value); }

		//private Coordinates _coord = new CheckCoordinates();
		[CommandOnMsg(nameof(CheckCoord), MessageType.Text, typeUser: TypeUser.User)]
		public TransactionCommandMessage GettAllCoordinates(IMessage msg)
		{
			var result = new List<CommandMessage>();
			return new TransactionCommandMessage(Coordinates.GetCoords(msg.Text).Select(CommandMessageWithDescription).ToList());
		}

		private CommandMessage CommandMessageWithDescription(Coordinate coord) => 
			CommandMessage.GetCoordMsg(coord, _coordinates.GetUrlLink(coord, true));
		
		[Command(nameof(Coords), "Скинуть в чат координаты из координат сообщения.")]
		public TransactionCommandMessage Coords(IMessage msg) => 
			GetCoord(msg, nameof(Coords), Coordinates.GetCoords);

		[Command(nameof(CoordsT), "Скинуть в чат координаты из координаты.")]
		public TransactionCommandMessage CoordsT(IMessage msg) =>
			GetCoord(msg, null, _coordinates.GetTextCoord);

		[Command(nameof(Route), "Построить маршрут по координатам.")]
		public CommandMessage Route(IMessage msg) =>  
			GetCommand(msg, null, _coordinates.GetPointes, !AddPicture);

		[Command(nameof(CreateMap), "Построить маршрут по строкам.")]
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
				CommandMessage.GetTextMsg(new Texter(func(text), true));
			}
			result.OnIdMessage = msg.MessageId;
			return result;
		}

		private TransactionCommandMessage GetCoord(IMessage msg, string fName, Func<string, IEnumerable<Coordinate>> func)
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
		}

	}
}
