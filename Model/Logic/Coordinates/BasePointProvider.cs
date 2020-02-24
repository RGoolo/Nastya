using System.Collections.Generic;
using Model.Logic.Settings;

namespace Model.Logic.Coordinates
{
	public abstract class BasePointProvider : IPointProvider<Place>, IPointProvider<Coordinate>
	{
		protected ICoordinates coordinates;
		protected static string GetUrl(string link, string name) => $"<a href=\"{link}\">{name}</a>";

		protected BasePointProvider(ICoordinates coordinates)
		{
			this.coordinates = coordinates;
		}

		public bool Use => coordinates.Show;

		public string GetUrl(Coordinate place) => GetPrivateUrl(place);

		public string GetUrl(List<Coordinate> places, TypePoints type) => GetPrivateUrl(places, type);

		public string GetUrl(Place place) => GetPrivateUrl(place);

		public string GetUrl(List<Place> places, TypePoints type) => GetPrivateUrl(places, type);

		protected abstract string GetPrivateUrl<T>(T point) where T : Point;

		protected abstract string GetPrivateUrl<T>(List<T> points, TypePoints type) where T : Point;

	}
}