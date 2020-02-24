using System.Globalization;

namespace Model.Logic.Coordinates.RegExp
{
	public static class ConvertDegree
	{
		public static string FirstMinus => "firstMinus";
		public static string SecondMinus => "secondMinus";

		public static float FromDegree(string i, string j, string k, string l, bool minus) =>
			FromDegree(Parse(i), Parse(j), Parse(k) + Parse((string.IsNullOrEmpty(l) ? "" : $".{l}")),
				minus ? -1 : 1);

		public static float FromDegree(float i, float j, float k, int minus) =>
			(i + j / 60 + k / 60 / 60) * (minus);

		public static float Parse(string str) => string.IsNullOrEmpty(str)
			? 0
			: float.Parse(str, NumberStyles.Any, CultureInfo.InvariantCulture);
	}
}