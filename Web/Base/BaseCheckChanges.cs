using System;

namespace Web.Base
{
	public static class BaseCheckChanges
	{
		public static bool IsBorderValue(TimeSpan? dt1, TimeSpan? dt2, int second)
		{
			if (!dt1.HasValue || !dt2.HasValue)
				return false;

			TimeSpan maxDt;
			TimeSpan minDt;
			if (dt1 > dt2)
			{
				maxDt = dt1.Value;
				minDt = dt2.Value;
			}
			else
			{
				maxDt = dt2.Value;
				minDt = dt1.Value;
			}

			return (maxDt.TotalSeconds >= second && minDt.TotalSeconds < second);
		}
	}
}