using System.Collections.Generic;
using System.Linq;
using BotModel.Bots.BotTypes;
using Model.Settings;

namespace Model.Logic.Coordinates
{
	public class YandexPointProvider : BasePointProvider
	{
		private readonly IChatService _settings;
		public static string yandexCity = @"ll={0}&";
		public static string yandexUrl = @"https://yandex.ru/maps/?{1}mode=routes&rtext={0}&z=12";

		public Coordinate GetCoordinate(string str)
		{
			var point = Yandex.YandexGeocoder.Geocode(str)?.FirstOrDefault()?.Point;
			return point == null ? null : new Coordinate(point.Value, str); ;
		}


		public YandexPointProvider(IChatService settings) : base(settings.Coordinates.Yandex)
		{
			_settings = settings;
		}


		protected override string GetPrivateUrl<T>(T point)  => GetPrivateUrl(point.ToString(), TypePoints.FromMe, coordinates.Name);

		protected override string GetPrivateUrl<T>(List<T> places, TypePoints type)=> GetPrivateUrl(string.Join("~", places.Select(x => x.ToString())), type);

		private string GetPrivateUrl(string text, TypePoints type, string name = null)
		{
			name ??= (type == TypePoints.FromMe ? coordinates.PointNameMe : coordinates.PointName);
			text = (type == TypePoints.FromMe ? "~" : "") + text;

			return GetUrl(string.Format(yandexUrl, text,
				_settings.Coordinates.City != null
					? string.Format(yandexCity, _settings.Coordinates.City)
					: string.Empty), name);
		}
	}
}