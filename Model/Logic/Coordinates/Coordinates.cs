using Model.Logic.Google;
using Model.Types.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Text;
using Model.Logic.Yandex;

namespace Model.Logic.Coordinates
{
    public class PointsFactory
    {
        private SettingsPoints _settings;
        private readonly string _creds;
        private readonly IFileWorker _fileWorker;
        private GooglePointProvider GooglePlacesProvider;
        private readonly YandexPointProvider _yandexPlacesProvider;
        private List<IPointProvider<Coordinate>> CoordinatesProvider;
        private List<IPointProvider<Place>> PlacesProvider;
        private ICoordinateWorker coordinateWorker;

        public PointsFactory(SettingsPoints settings, string creds, IFileWorker fileWorker)
        {
            _settings = settings;
            _creds = creds;
            _fileWorker = fileWorker;
            GooglePlacesProvider = new GooglePointProvider(settings, creds);
            _yandexPlacesProvider = new YandexPointProvider(settings);

            CoordinatesProvider = new List<IPointProvider<Coordinate>>() {GooglePlacesProvider, _yandexPlacesProvider};
            PlacesProvider = new List<IPointProvider<Place>>() { GooglePlacesProvider, _yandexPlacesProvider };
        }


        public PointWorker<Coordinate> GetCoordinates(string text) 
        {
            return new CoordinatesWorker(CoordinatesProvider.Where(x => x.Use).ToList(), text, coordinateWorker);
        }

        public PointWorker<Place> GetPlaces(string text)
        {
            return new PlacesWorker(PlacesProvider.Where(x => x.Use).ToList(), text, coordinateWorker);
        }

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


        private string GetPrivateUrl<T>(T point) where T : Point
        {
            return _startLink + $"&destination={point}";
        }

        private string GetPrivateUrl<T>(List<T> points, TypePoints type) where T : Point
        {
            StringBuilder sb = new StringBuilder(_startLink);
            var origin = type == TypePoints.FromMe;
            if (origin)
                sb.Append($"&origin={points.First()}");

            sb.Append($"&destination={points.Last()}");

            if (points.Count > 1)
            {
                var points1 = points.Select(x => x.ToString()).SkipLast(origin ? 1 : 0).Aggregate((x, y) => x + "%7C" + y);
                sb.Append($"&waypoints={points1}");
            }
            return sb.ToString();
        }
    }

    public class YandexPointProvider : IPointProvider<Place>, IPointProvider<Coordinate>
    {
        public static string yandexCity = @"ll={0}&";
        public static string yandexUrl = @"https://yandex.ru/maps/?{1}mode=routes&rtext={0}&z=12";

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

        private string GetPrivateUrl<T>(T point) where T : Point => GetPrivateUrl(point.ToString(), TypePoints.FromMe);

        public string GetPrivateUrl<T>(List<T> places, TypePoints type) where T : Point => GetPrivateUrl(string.Join("~", places.Select(x => x.ToString())), type);

        public string GetPrivateUrl(string text, TypePoints type)
        {
            text = (type == TypePoints.FromMe ? "~" : "") + text;

            return string.Format(yandexUrl, text,
                _settings.City != null
                    ? string.Format(yandexCity, _settings.City)
                    : string.Empty);
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

        protected abstract List<T> GetPoints();

        protected PointWorker(List<IPointProvider<T>> providers, string text, ICoordinateWorker coordinateWorker)
        {
            Providers = providers;
            Text = text;
            CoordinateWorker = coordinateWorker;
        }

        public List<T> Points => _points ?? (_points = GetPoints());

        public string ReplacePoints()
        {
            if (Providers.Count == 0 || Points.Count == 0) return Text;
            
            var points = Points;
            points.ForEach(coordinate => coordinate.Urls = string.Join(string.Empty, Providers.Select(x => x.GetUrl(coordinate))));

            if (points.Count > 1)
                points.Last().Urls += string.Join(String.Empty, Providers.Select(x => x.GetUrl(points, TypePoints.FromMe)))
                                           + string.Join(String.Empty, Providers.Select(x => x.GetUrl(points, TypePoints.OnlyPoint)));

            var result = new StringBuilder(Text);
            Points.ForEach(coordinate => result.Replace(coordinate.OriginText, coordinate.OriginText + coordinate.Urls));
            return result.ToString();
        }
    }
}
