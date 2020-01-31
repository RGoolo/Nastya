using Model.Logic.Google;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model.Files.FileTokens;

namespace Model.Logic.Coordinates
{
	public class PointsFactory
	{
		private SettingsPoints _settings;
		private readonly string _creds;
		private GooglePointProvider GooglePlacesProvider;
		private readonly YandexPointProvider _yandexPlacesProvider;
		private List<IPointProvider<Coordinate>> CoordinatesProvider;
		private List<IPointProvider<Place>> PlacesProvider;
		private readonly ICoordinateWorker _coordinateWorker;

		public PointsFactory(SettingsPoints settings, string creds)
		{
			_settings = settings;
			var _creds = creds;
			// _fileWorker = fileWorker;
			GooglePlacesProvider = new GooglePointProvider(settings, _creds);
			_yandexPlacesProvider = new YandexPointProvider(settings);

			CoordinatesProvider = new List<IPointProvider<Coordinate>>() {GooglePlacesProvider, _yandexPlacesProvider};
			PlacesProvider = new List<IPointProvider<Place>>() { GooglePlacesProvider, _yandexPlacesProvider };
		}

		public Coordinate To(Place place) => GetCoordinate(place.OriginText);

		public PointWorker<Coordinate> GetCoordinates(string text) => new CoordinatesWorker(CoordinatesProvider.Where(x => x.Use).ToList(), text, _coordinateWorker);

		public void SetPicture(IChatFile file, IEnumerable<Point> points) => new FactoryMaps(_creds).SaveImg(file, points);

		public PointWorker<Place> GetPlaces(string text) => new PlacesWorker(PlacesProvider.Where(x => x.Use).ToList(), text, _coordinateWorker);

		public Coordinate GetCoordinate(string str) => _yandexPlacesProvider.GetCoordinate(str);

		/*public Coordinate GetCoordinate(string str)
		{
			//var point = Yandex.YandexGeocoder.Geocode(str)?.FirstOrDefault()?.Point;
			//return point == null ? null : new Coordinate(point.Value, str); ;
		}

		public string GetPlace(string str) => Yandex.YandexGeocoder.Geocode(str)?.FirstOrDefault()?.GeocoderMetaData.Text ?? string.Empty + " ";
		public void GetPicture(string str, IFileToken file) => _factoryMaps.SaveImg(file, new Maps(CoordinatesChecker.GetCoords(str)));
		public void GetPictureText(string str, IFileToken file) => _factoryMaps.SaveImg(file, new Maps(_splitString(str)));*/
	}

	public enum TypePoints
	{
		FromMe, OnlyPoint
	}

	public class GooglePointProvider : IPointProvider<Place>, IPointProvider<Coordinate>
	{
		private static string GetUrl(string link, string name) => $"<a href=\"{link}\">{name}</a>";

		private static string _startLink = @"https://www.google.com/maps/dir/?api=1";
		public TravelMode TravelMode = TravelMode.driving;
		//public List<Point> WayPoints = new List<string>();

		public GooglePointProvider(SettingsPoints settings, string cred)
		{
			_settings = settings;
			_cred = cred;
		}

		private SettingsPoints _settings;
		private readonly string _cred;

		public bool Use => _settings.Google.LinkFor;

		public string GetUrl(Coordinate place) => GetPrivateUrl(place);

		public string GetUrl(List<Coordinate> places, TypePoints type) => GetPrivateUrl(places, type);

		public string GetUrl(Place place) => GetPrivateUrl(place);

		public string GetUrl(List<Place> places, TypePoints type) => GetPrivateUrl(places, type);

		private string GetPrivateUrl<T>(T point) where T : Point => GetUrl(_startLink + $"&destination={point}", _settings.Google.Name);

		private string GetPrivateUrl<T>(List<T> points, TypePoints type) where T : Point
		{
			var name = (type == TypePoints.FromMe ? _settings.Google.PointNameMe : _settings.Google.PointName);
			StringBuilder sb = new StringBuilder(_startLink);
			var origin = type == TypePoints.OnlyPoint;

			if (origin)
				sb.Append($"&origin={points.First()}");

			sb.Append($"&destination={points.Last()}");

			if ((origin && points.Count > 2) || (!origin && points.Count > 1))
			{
				var points1 = points.Select(x => x.ToString()).SkipLast(1).Skip(origin ? 1 : 0).Aggregate((x, y) => x + "%7C" + y);
				sb.Append($"&waypoints={points1}");
			}
			return GetUrl(sb.ToString(), name);
		}
	}

	public class YandexPointProvider : IPointProvider<Place>, IPointProvider<Coordinate>
	{
		public static string yandexCity = @"ll={0}&";
		public static string yandexUrl = @"https://yandex.ru/maps/?{1}mode=routes&rtext={0}&z=12";

		private static string GetUrl(string link, string name) => $"<a href=\"{link}\">{name}</a>";

		public Coordinate GetCoordinate(string str)
		{
			var point = Yandex.YandexGeocoder.Geocode(str)?.FirstOrDefault()?.Point;
			return point == null ? null : new Coordinate(point.Value, str); ;
		}


		public YandexPointProvider(SettingsPoints settings)
		{
			_settings = settings;
		}

		private readonly SettingsPoints _settings;

		public bool Use => _settings.Yandex.LinkFor;

		public string GetUrl(Coordinate place) => GetPrivateUrl(place);

		public string GetUrl(List<Coordinate> places, TypePoints type) => GetPrivateUrl(places, type);

		public string GetUrl(Place place) => GetPrivateUrl(place);

		public string GetUrl(List<Place> places, TypePoints type) => GetPrivateUrl(places, type);

		private string GetPrivateUrl<T>(T point) where T : Point => GetPrivateUrl(point.ToString(), TypePoints.FromMe, _settings.Yandex.Name);

		public string GetPrivateUrl<T>(List<T> places, TypePoints type) where T : Point => GetPrivateUrl(string.Join("~", places.Select(x => x.ToString())), type);

		public string GetPrivateUrl(string text, TypePoints type, string name = null)
		{
			name ??= (type == TypePoints.FromMe ? _settings.Yandex.PointNameMe : _settings.Yandex.PointName);
			text = (type == TypePoints.FromMe ? "~" : "") + text;

			return GetUrl(string.Format(yandexUrl, text,
				_settings.City != null
					? string.Format(yandexCity, _settings.City)
					: string.Empty), name);
		}
	}

	public interface IPointProvider<T> where T : Point
	{
		bool Use { get; }
		string GetUrl(T place);
		string GetUrl(List<T> places, TypePoints type);
	}

	public interface ICoordinateWorker
	{
		string GetMaps(List<Point> coordinates);
	}

	public class PlacesWorker : PointWorker<Place>
	{
		public PlacesWorker(List<IPointProvider<Place>> providers, string text, ICoordinateWorker coordinateWorker) : base(providers, text, coordinateWorker)
		{

		}

		protected override List<Place> GetPoints() => RegExPoints.GetPlaces(Text).ToList();
	}

	public class CoordinatesWorker : PointWorker<Coordinate>
	{
		public CoordinatesWorker(List<IPointProvider<Coordinate>> providers, string text, ICoordinateWorker coordinateWorker) : base(providers, text, coordinateWorker)
		{

		}

		protected override List<Coordinate> GetPoints() => RegExPoints.GetCoords(Text).ToList();
	}

	public abstract class PointWorker<T> where T : Point
	{
		internal readonly List<IPointProvider<T>> Providers;
		internal readonly string Text;
		internal readonly ICoordinateWorker CoordinateWorker;
		private List<T> _points;
		private string _totalPoints;
		protected abstract List<T> GetPoints();

		protected PointWorker(List<IPointProvider<T>> providers, string text, ICoordinateWorker coordinateWorker)
		{
			Providers = providers;
			Text = text;
			CoordinateWorker = coordinateWorker;
		}

		public List<T> Points()
		{
			if (_points != null)
				return _points;

			_points = GetPoints();
			_points.ForEach(coordinate => coordinate.Urls = string.Join(string.Empty, Providers.Select(x => x.GetUrl(coordinate))));

		

			return _points;
		}

		public string TotalPoints() => _totalPoints 
			??= _totalPoints = 
				string.Join(string.Empty, Providers.Select(x => x.GetUrl(Points(), TypePoints.FromMe)))
				+ string.Join(string.Empty, Providers.Select(x => x.GetUrl(Points(), TypePoints.OnlyPoint)));

		public string ReplacePoints()
		{
			if (Providers.Count == 0 || Points().Count == 0) return Text;

			var points = Points();
			
			var result = new StringBuilder(Text);
			foreach (var coordinate in points.SkipLast(1))
				result.Replace(coordinate.OriginText, coordinate.OriginText + coordinate.Urls);

			result.Replace(points.Last().OriginText, points.Last().Urls + TotalPoints());
			return result.ToString();
		}
	}
}
