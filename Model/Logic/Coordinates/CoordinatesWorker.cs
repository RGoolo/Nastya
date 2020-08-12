using System.Collections.Generic;
using System.Linq;
using BotModel.Bots.BotTypes;
using Model.Logic.Coordinates.RegExp;

namespace Model.Logic.Coordinates
{
	public class CoordinatesWorker : PointWorker<Coordinate>
	{
		public CoordinatesWorker(List<IPointProvider<Coordinate>> providers, string text) : base(providers, text)
		{

		}

		protected override List<Coordinate> GetPoints() => RegExPoints.GetCoords(Text).ToList();
	}
}