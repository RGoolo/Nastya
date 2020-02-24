using System.Collections.Generic;
using System.Linq;
using Model.Logic.Coordinates.RegExp;

namespace Model.Logic.Coordinates
{
	public class CoordinatesWorker : PointWorker<Coordinate>
	{
		public CoordinatesWorker(List<IPointProvider<Coordinate>> providers, string text, ICoordinateWorker coordinateWorker) : base(providers, text, coordinateWorker)
		{

		}

		protected override List<Coordinate> GetPoints() => RegExPoints.GetCoords(Text).ToList();
	}
}