using System.Collections.Generic;
using System.Linq;
using System.Text;
using BotModel.Bots.BotTypes;

namespace Model.Logic.Coordinates
{
	public abstract class PointWorker<T> where T : Point
	{
		internal readonly List<IPointProvider<T>> Providers;
		internal readonly string Text;
		private List<T> _points;
		private string _totalPoints;
		protected abstract List<T> GetPoints();

		protected PointWorker(List<IPointProvider<T>> providers, string text)
		{
			Providers = providers;
			Text = text;
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
				result.Replace(coordinate.OriginText,  $"[{coordinate.Alias}] {coordinate.OriginText}{coordinate.Urls}");
			var pointLast = points.Last();
			result.Replace(pointLast.OriginText, $"[{pointLast.Alias}] " + pointLast.OriginText + pointLast.Urls + TotalPoints());
			return result.ToString();
		}
	}
}