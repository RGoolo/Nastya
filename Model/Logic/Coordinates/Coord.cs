using Model.Logic.Yandex;

namespace Model.Logic.Coordinates
{
	public class Coordinate
	{
		public float Latitude;
		public float Longitude;
		public string OriginText;

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
}
