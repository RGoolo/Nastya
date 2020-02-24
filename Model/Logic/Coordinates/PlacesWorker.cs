using System.Collections.Generic;
using System.Linq;
using Model.Logic.Coordinates.RegExp;

namespace Model.Logic.Coordinates
{
	public class PlacesWorker : PointWorker<Place>
	{
		public PlacesWorker(List<IPointProvider<Place>> providers, string text, ICoordinateWorker coordinateWorker) : base(providers, text, coordinateWorker)
		{

		}

		protected override List<Place> GetPoints() => RegExPoints.GetPlaces(Text).ToList();
	}
}