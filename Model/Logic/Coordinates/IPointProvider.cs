using System.Collections.Generic;

namespace Model.Logic.Coordinates
{
	public interface IPointProvider<T> where T : Point
	{
		bool Use { get; }
		string GetUrl(T place);
		string GetUrl(List<T> places, TypePoints type);
	}
}