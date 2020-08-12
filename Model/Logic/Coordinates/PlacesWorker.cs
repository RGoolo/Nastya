using System.Collections.Generic;
using System.Linq;
using BotModel.Bots.BotTypes;
using Model.Logic.Coordinates.RegExp;

namespace Model.Logic.Coordinates
{
	public class PlacesWorker : PointWorker<Place>
	{
		public PlacesWorker(List<IPointProvider<Place>> providers, string text) : base(providers, text)
		{

		}

		protected override List<Place> GetPoints() => RegExPoints.GetPlaces(Text).ToList();
	}
}