using System;

namespace Model.Bots.BotTypes.Class
{
	public static class IdsMapper
	{
		public static Guid ToGuid(long id) => new Guid((int)id, (short)(id >> 8 * 4), (short)(id >> 8 * 6), 0, 0, 0, 0, 0, 0, 0, 0);

		public static Guid ToGuid(int id) => new Guid(id, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

		public static long ToLong(Guid guid) => ToLong(guid, 8);

		public static int ToInt(Guid guid) => (int)ToLong(guid, 4);
		public static int ToInt(Guid? guid) => guid == null ? default : ToInt(guid.Value);
		
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