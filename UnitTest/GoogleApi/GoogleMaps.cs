using Model.Logic.Coordinates;
using Model.Logic.Google;
using Model.Logic.Settings;
using Model.Types.Class;
using Model.Types.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace UnitTest.GoogleApi
{
	public class GoogleMaps
	{


		[Fact]
		public void Test()
		{
	
			var password = SecurityEnvironment.GetTextPassword("google_maps_token");
			var fileWorker = new LocalFileWorker(Guid.Empty);

			var worker = new LocalFileWorker(Guid.Empty);
			var file = worker.NewFileTokenByExt(".jpg");
			var factoryMaps = new FactoryMaps(password, fileWorker);

			var points = new List<Point>
			{
				new Coordinate(59.9225543f, 30.337371f, "Вокзал."),
				new Place("Площадь ленина 1"),
			};
			factoryMaps.SaveImg(file, points);
			Console.WriteLine(file.FileName);
		}

	}
}
