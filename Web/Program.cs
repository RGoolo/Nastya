using System;
using Ts = System.Timers;
using System.Text;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using Model.Types.Class;

namespace Web
{
	class Program
	{
		static void Main(string[] args)
		{
			new CheckTimer();
			Console.ReadLine();
		}


		static void SetPage(string file)
		{

			//var file = @"dzzzr\DozoR.Night _ КРЕВЕТКА+НАУКА=ДЕНИАЛИЗМ.html";
			string str = null;
			using (var stream = new StreamReader(file, Encoding.GetEncoding(1251)))
				str = stream.ReadToEnd();

			//string  = ((Settings.Settings.TypeGame & TypeGame.Dummy) == TypeGame.Dummy) ? string.Empty : GetUrl();
			DZR.DzrPage p = new DZR.DzrPage(str, "C:\\");
		}
	}

	public class CheckTimer
	{
		private readonly System.Timers.Timer _refreshTimer;

		public CheckTimer()
		{
			_refreshTimer = new System.Timers.Timer(1000);
			_refreshTimer.Elapsed += _refreshTimer_Elapsed;
			_refreshTimer.Start();
		}

		private static int i = 0;

		private void _refreshTimer_Elapsed(object sender, Ts.ElapsedEventArgs e)
		{
			i++;
			var j = i;
			Console.WriteLine(j);
			Thread.Sleep(3000);
			Console.WriteLine(j);
		}
	}
}
