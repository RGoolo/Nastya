using System.Collections.Generic;
using BotModel.Bots.BotTypes;

namespace Model.Logic.Coordinates
{
	public interface ICoordinateWorker
	{
		string GetMaps(List<Point> coordinates);
	}
}