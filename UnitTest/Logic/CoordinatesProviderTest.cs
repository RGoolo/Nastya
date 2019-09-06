using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Model.Logic.Coordinates;
using Model.Logic.Settings;
using Model.Types.Interfaces;
using Xunit;

namespace UnitTest.Logic
{
    public class CoordinatesProviderTest
    {

      

        [Theory]
        [MemberData(nameof(OneCoordinate))]
        public void OneCoordinateTest(string coords)
        {
            var factory = new PointsFactory(new SettingsPoints(), string.Empty, new LocalFileWorker(Guid.Empty));
            var point = factory.GetCoordinates(coords);
            var str = point.ReplacePoints();
            var str2 = str;
        }

        public static IEnumerable<object[]> OneCoordinate()
        {
      
            yield return new object[] { "N55°45'20\".992, E37°36'43\".200  — градусы(+ доп.буквы)\nN55°45'20\".992, E37°36'43\".200  — градусы(+ доп.буквы)" };
        }
        public static IEnumerable<object[]> OnePlace()
        {
            yield return new object[] { "Невский проспект, 85;Санкт-Петербург (Финляндский вокзал);Витебский вокзал" };
        }

        [Theory]
        [MemberData(nameof(OnePlace))]
        public void OnePlaceTest(string coords)
        {
            var factory = new PointsFactory(new SettingsPoints(), string.Empty, new LocalFileWorker(Guid.Empty));
            var point = factory.GetPlaces(coords);
            var str = point.ReplacePoints();
            var str2 = str;
        }

    }
}