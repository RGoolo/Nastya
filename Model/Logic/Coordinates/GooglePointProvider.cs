using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model.Logic.Google;
using Model.Logic.Settings;

namespace Model.Logic.Coordinates
{
	public class GooglePointProvider : BasePointProvider
	{
	
		private static string _startLink = @"https://www.google.com/maps/dir/?api=1";
		public TravelMode TravelMode = TravelMode.driving;
		//public List<Point> WayPoints = new List<string>();

		public GooglePointProvider(IChatService settings) : base(settings.Coordinates.Google)
		{
	
		}

		protected override string GetPrivateUrl<T>(T point) => GetUrl(_startLink + $"&destination={point}", coordinates.Name);

		protected override string GetPrivateUrl<T>(List<T> points, TypePoints type)
		{
			var name = (type == TypePoints.FromMe ? coordinates.PointNameMe : coordinates.PointName);
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
}