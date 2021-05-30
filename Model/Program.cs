using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Model
{
	class Program
	{
		public static void Main(string[] args)
		{
			
		}

		public void Test(IEnumerable<string> coll)
		{
			var a = coll.FirstOrDefault();
			var b = coll.ToList();
		}
	}
}