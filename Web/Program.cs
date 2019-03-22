using Ts = System.Timers;
using System.Text;
using System.IO;

namespace Web
{
	class Program
	{
		static void Main(string[] args)
		{

		}

		static void SetPage(string file)
		{
			//var file = @"dzzzr\DozoR.Night _ КРЕВЕТКА+НАУКА=ДЕНИАЛИЗМ.html";
			string str = null;
			using (var stream = new StreamReader(file, Encoding.GetEncoding(1251)))
				str = stream.ReadToEnd();
			
			//string  = ((Settings.Settings.TypeGame & TypeGame.Dummy) == TypeGame.Dummy) ? string.Empty : GetUrl();
			DZR.Page p = new DZR.Page(str, "C:\\");
		}

	}
}
