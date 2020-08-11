using System.ComponentModel;
using Model.Bots.BotTypes.Attribute;
using Model.Bots.BotTypes.Enums;
using Model.Logic.Coordinates;
using Model.Logic.Settings;

namespace Nastya.Commands
{
	[CommandClass("coordsSettings", "Работа с координатами.", TypeUser.User)]
	public class CoordinateSettings
	{
		private readonly IChatService _settings;
		private IPointsFactory _pointsFactory;

		public CoordinateSettings(IChatService settings)
		{
			_settings = settings;

			//ToDo ubdate
			var cred = settings.Coordinates.GoogleCred;
			_pointsFactory = settings.PointsFactory;
		}

		[Command(Const.Coordinates.Yandex.NameLink, "Имя ссылки на Yandex карты.")]
		public string NameYLink { get; set; }
		
		[Command(Const.Coordinates.Yandex.NamePoints, "Имя ссылки на Yandex маршрут от меня.")]
		public string NameYPoints { get; set; }
		
		[Command(Const.Coordinates.Yandex.NamePointsMe, "Имя ссылки на Yandex маршрут.")]
		public string NameYPointsMe { get; set; }

		[Command(Const.Coordinates.Yandex.Show, "Добавлять в координаты ссылку на yandex map.")]
		public bool AddYandex { get; set; }

		[Command(Const.Game.City, "Установить город, для карт.")]
		public void City([Description("Город")] string city)
		{
			_settings.Coordinates.City = _pointsFactory.GetCoordinate(city).ToString();
		}


		[Command(Const.Coordinates.Google.NameLink, "Имя ссылки на Google карты.")]
		public string NameGLink { get; set; }
		

		[Command(Const.Coordinates.Google.NamePoints, "Имя ссылки на Google маршрут от меня.")]
		public string NameGPoints { get; set; }

		[Command(Const.Coordinates.Google.NamePointsMe, "Имя ссылки на Google точки.")]
		public string NameGPointsMe { get; set; }

		/*	[Command(nameof(Dontaddkml), "Не добавляет kml файл к сообщениям на координаты.", "{F8F5407E-64A7-483A-82C6-FA26740ABB48}")]
		public bool Dontaddkml{ get; set; }*/

		[Command(Const.Coordinates.Google.Show, "Добавлять в координаты ссылку на google map.")]
		public bool AddGoogle { get; set; }
	}
}