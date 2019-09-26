using Model.Logic.Yandex;

namespace Model.Logic.Coordinates
{

	public enum TypePoint
	{
		Coordinate, Place
	}

	public abstract class Point
	{
		public abstract TypePoint TypePoint { get; }
		public abstract string OriginText { get; }
		public string Urls { get; set; }
	}

	public class Coordinate : Point
	{
		public float Latitude { get; }
		public float Longitude{ get; }
		public override string OriginText { get;}
		public override TypePoint TypePoint => TypePoint.Coordinate;

		public Coordinate(float lat , float @long, string originText)
		{
			Latitude = lat;
			Longitude = @long;
			OriginText = originText;
		}

		public Coordinate(GeoPoint point, string originText)
		{
			Latitude = (float)point.Lat;
			Longitude = (float)point.Long;
			OriginText = originText;
		}

		public override string ToString() => $"{Latitude},{Longitude}";
	}

	public class Place : Point
	{
		public override string OriginText { get; }

		public Place(string originText)
		{
			OriginText  = originText;
		}

		public override string ToString() => OriginText;
		public override TypePoint TypePoint => TypePoint.Place;
	}
}
