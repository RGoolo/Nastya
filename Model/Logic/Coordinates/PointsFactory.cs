using Model.Logic.Google;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Model.Files.FileTokens;
using Model.Logic.Settings;

namespace Model.Logic.Coordinates
{
	public interface IPointsFactory
	{
		Coordinate To(Place place);
		PointWorker<Coordinate> GetCoordinates(string text);
		Task SetPicture(IChatFile file, IEnumerable<Point> points);
		PointWorker<Place> GetPlaces(string text);
		Coordinate GetCoordinate(string str);
	}

	public class PointsFactory : IPointsFactory
	{
		private ISettings _settings;
		private GooglePointProvider GooglePlacesProvider;
		private readonly YandexPointProvider _yandexPlacesProvider;
		private List<IPointProvider<Coordinate>> CoordinatesProvider;
		private List<IPointProvider<Place>> PlacesProvider;

		private string creads => _settings.Coordinates.GoogleCreads;

		public PointsFactory(ISettings settings)
		{
			_settings = settings;

			// _fileWorker = fileWorker;
			GooglePlacesProvider = new GooglePointProvider(settings);
			_yandexPlacesProvider = new YandexPointProvider(settings);

			CoordinatesProvider = new List<IPointProvider<Coordinate>>() {GooglePlacesProvider, _yandexPlacesProvider};
			PlacesProvider = new List<IPointProvider<Place>>() { GooglePlacesProvider, _yandexPlacesProvider };
		}

		public Coordinate To(Place place) => GetCoordinate(place.OriginText);

		public PointWorker<Coordinate> GetCoordinates(string text) => new CoordinatesWorker(CoordinatesProvider.Where(x => x.Use).ToList(), text);

		public Task SetPicture(IChatFile file, IEnumerable<Point> points) => new GoogleImgForMaps(creads).SaveImg(file, points);

		public PointWorker<Place> GetPlaces(string text) => new PlacesWorker(PlacesProvider.Where(x => x.Use).ToList(), text);

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
}
