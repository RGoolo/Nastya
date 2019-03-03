using System;
using System.Linq;

namespace Model.Types.Class
{
	public static class IdsMapper
	{
		public static Guid ToGuid(this long id) => new Guid((int)id, (short)(id >> 8 * 4), (short)(id >> 8 * 6), 0, 0, 0, 0, 0, 0, 0, 0);

		public static Guid ToGuid(this int id) => new Guid(id, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

		public static long ToLong(this Guid guid) => ToLong(guid, 8);

		public static int ToInt(this Guid guid) => (int)ToLong(guid, 4);
		
		private static long ToLong(Guid guid, byte shift)
		{
			long res = 0;
			var bytes = guid.ToByteArray();
			
			for (int i = 0; i < shift; i++)
				res += ((long)bytes[i] << i * 8);
	
			return res;
		}

	}
}