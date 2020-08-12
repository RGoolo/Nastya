using System.Collections;
using System.Collections.Generic;
using System.Xml;
using BotModel.Bots.BotTypes.Class;

namespace Model.Logic.Yandex
{
	public class GeoObjectCollection : IEnumerable<GeoObject>
	{
		readonly List<GeoObject> _geoObjects;

		public GeoObjectCollection(string xml)
		{
			_geoObjects = new List<GeoObject>();
			ParseXml(xml);
		}

		private void ParseXml(string xml)
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(xml);

			XmlNamespaceManager ns = new XmlNamespaceManager(doc.NameTable);
			ns.AddNamespace("ns", "http://maps.yandex.ru/ymaps/1.x");
			ns.AddNamespace("opengis", "http://www.opengis.net/gml");
			ns.AddNamespace("geocoder", "http://maps.yandex.ru/geocoder/1.x");

			XmlNodeList nodes = doc.SelectNodes("//ns:ymaps/ns:GeoObjectCollection/opengis:featureMember/ns:GeoObject", ns);
			foreach (XmlNode node in nodes)
			{
				var pointNode = node.SelectSingleNode("opengis:Point/opengis:pos", ns);
				var boundsNode = node.SelectSingleNode("opengis:boundedBy/opengis:Envelope", ns);
				var metaNode = node.SelectSingleNode("opengis:metaDataProperty/geocoder:GeocoderMetaData", ns);

				GeoObject obj = new GeoObject
				{
					Point = pointNode == null ? new GeoPoint() : GeoPoint.Parse(pointNode.InnerText),
					BoundedBy = boundsNode == null ? new GeoBound() : new GeoBound(
						GeoPoint.Parse(boundsNode["lowerCorner"].InnerText), GeoPoint.Parse(boundsNode["upperCorner"].InnerText)
						),
					GeocoderMetaData = new GeoMetaData(metaNode["text"].InnerText, metaNode["kind"].InnerText)
				};
				_geoObjects.Add(obj);
			}
		}

		public GeoObject this[int i]
		{
			get
			{
				return _geoObjects[i];
			}
		}

		public IEnumerator<GeoObject> GetEnumerator() => _geoObjects.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => _geoObjects.GetEnumerator();
	}

	public class GeoObject
	{
		public GeoPoint Point;
		public GeoBound BoundedBy;
		public GeoMetaData GeocoderMetaData;
	}

	public class GeoMetaData
	{
		public KindType Kind = KindType.locality;
		public string Text = string.Empty;
		public GeoMetaData(string text, string kind)
		{
			Text = text;
			Kind = ParseKind(kind);
		}

		public static KindType ParseKind(string kind)
		{
			switch (kind)
			{
				case "district": return KindType.district;
				case "house": return KindType.house;
				case "locality": return KindType.locality;
				case "metro": return KindType.metro;
				case "street": return KindType.street;
				default: return KindType.locality;
			}
		}

		public override string ToString()
		{
			return Text;
		}
	}

    public struct GeoBound
	{
		public GeoPoint LowerCorner, UpperCorner;
		public GeoBound(GeoPoint lowerCorner, GeoPoint upperCorner)
		{
			LowerCorner = lowerCorner;
			UpperCorner = upperCorner;
		}

		public override string ToString()
		{
			return string.Format("[{0}] [{1}]", LowerCorner.ToString(), UpperCorner.ToString());
		}
	}

	public struct SearchArea
	{
		public GeoPoint LongLat, Spread;
		public SearchArea(GeoPoint centerLongLat, GeoPoint spread)
		{
			LongLat = centerLongLat;
			Spread = spread;
		}
	}
}
