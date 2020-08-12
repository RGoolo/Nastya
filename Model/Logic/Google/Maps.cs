using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BotModel.Bots.BotTypes;
using BotModel.Files.FileTokens;

namespace Model.Logic.Google
{

	public enum TravelMode
	{
		driving, walking, bicycling, transit
	}

	public class GoogleImgForMaps
	{
		public static string GetSearchPhotoUrl(string url) => $@"https://www.google.com/searchbyimage?image_url={url}";

		private static string _startImg = @"https://maps.googleapis.com/maps/api/staticmap?size=600x600";
		private string GoogleToken { get; }
		private string Key => $"key={GoogleToken}";

		public GoogleImgForMaps(string googleToken)
		{
			GoogleToken = googleToken;
		}

		protected string GetUrlImg(IEnumerable<Point> points)
		{
			return _startImg + "&" + string.Join("&", points.Select(x => new Marker(x, x.Alias).ToString())) + "&" + Key;
		}

		public async Task SaveImg(IChatFile file, IEnumerable<Point> points)
		{
			var urlImg = GetUrlImg(points);
			var request = WebRequest.Create(urlImg);
			using var response = await request.GetResponseAsync();
			await using var stream =  response.GetResponseStream();
			await using var fileStream = file.WriteStream();

			stream?.CopyTo(fileStream);
		}
	}

	public class Marker
	{
		private const string url = "&markers=color:{0}%7Clabel:{1}%7C{2}";
		public string Color { get; set; } = "green";
		public char Label { get; set; }
		public Point Point { get;}

		public Marker(Point point, char label)
		{
			Point = point;
			Label = label;
		}
		public override string ToString() => $"markers=color:{Color}%7Clabel:{Label}%7C{Point}";
	}
}
