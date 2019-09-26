using Model.Logic.Coordinates;
using Model.Types.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Model.Logic.Google
{

	public enum TravelMode
	{
		driving, walking, bicycling, transit
	}

	public class FactoryMaps
	{
		public static string GetSearchPhotoUrl(string url) => $@"https://www.google.com/searchbyimage?image_url={url}";


		private static string _startImg = @"https://maps.googleapis.com/maps/api/staticmap?size=600x600";
		private readonly IFileWorker _fileWorker;
		private string _googleToken { get; }
		private string Key => $"key={_googleToken}";

		public FactoryMaps(string googleToken, IFileWorker fileWorker) //
		{
			
			_googleToken = googleToken;
			_fileWorker = fileWorker;
		}

		protected string GetUrlImg(IEnumerable<Point> points)
		{
			var i = 0;
			return _startImg + "&" + string.Join("&", points.Select(x => new Marker(x, (++i).ToString()).ToString())) + "&" + Key;
		}

		public void SaveImg(IFileToken file, IEnumerable<Point> points)
		{
			var urlImg = GetUrlImg(points);
			Console.WriteLine(urlImg);
			Console.WriteLine("file:" + file.FileName);
			var request = WebRequest.Create(urlImg);
			using (var response = request.GetResponse())
			using (var stream = response.GetResponseStream())

			using (var fileStream = _fileWorker.WriteStream(file))
				stream.CopyTo(fileStream);
		}

		public static Maps GetMap(Coordinates.Coordinate coord)
		{
			return new Maps(new List<Coordinates.Coordinate> { coord });
		}

		public static Maps GetMap(string coord)
		{
			return new Maps(new List<string> { coord });
		}
		public static Maps GetMap(IEnumerable<Coordinates.Coordinate> coords)
		{
			return new Maps(coords);
		}
		public static Maps GetMap(IEnumerable<string> places)
		{
			return new Maps(places);
		}
	}

	public class Marker
	{

		private const string url = "&markers=color:{0}%7Clabel:{1}%7C{2}";
		public string Color { get; set; } = "green";
		public string Label { get; set; } = "point";
		public Point Point { get;}


		public Marker(Point point, string label)
		{
			Point = point;
			Label = label;
		}
		public override string ToString() => $"markers=color:{Color}%7Clabel:{Label}%7C{Point}";
	}


	public class Maps
	{
		private static string _startLink = @"https://www.google.com/maps/dir/?api=1";
		public TravelMode TravelMode = TravelMode.driving;
		public List<string> WayPoints = new List<string>();

		public bool isDir_action;

		public Maps(IEnumerable<Coordinates.Coordinate> coords)
		{
			if (!coords.Any())
				return;

			//Destination = coords.ElementAt(0);
			WayPoints = coords.Select(x => x.ToString()).ToList();
			//Destination = coords.Last().ToString();
		}

		public Maps(IEnumerable<string> places)
		{
			if (!places.Any())
				return;

			//Destination = coords.ElementAt(0);
			WayPoints = places.ToList();
		}

		public string ToString(bool origin = true)
		{
			StringBuilder sb = new StringBuilder(_startLink);

			if (origin)
				sb.Append($"&origin={WayPoints.First()}");

			sb.Append($"&destination={WayPoints.Last()}");

			if (WayPoints.Count > 1)
			{
				var points = WayPoints.SkipLast(origin ? 1 : 0).Aggregate((x, y) => x + "%7C" + y);
				sb.Append($"&waypoints={points}");
			}
			return sb.ToString();
		}
	}
}
