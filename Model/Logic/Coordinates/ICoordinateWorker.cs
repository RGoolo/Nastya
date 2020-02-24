using System.Collections.Generic;

namespace Model.Logic.Coordinates
{
	public interface ICoordinateWorker
	{
		string GetMaps(List<Point> coordinates);
	}
}