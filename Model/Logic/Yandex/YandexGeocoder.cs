using System.IO;
using System.Net;
using static System.String;

namespace Model.Logic.Yandex
{
	public static class YandexGeocoder
	{
		const string RequesrtUrl = "http://geocode-maps.yandex.ru/1.x/?geocode={0}&format=xml&results={1}&lang={2}";

		public static string GetSearchPhotoUrl(string url) => $@"https://yandex.ru/images/search?rpt=imageview&cbird=1&img_url={url}";

		static YandexGeocoder()
		{
			Key = Empty;
		}

		public static string Key { get; set; }

		public static GeoObjectCollection Geocode(string location) => Geocode(location, 10);

		public static GeoObjectCollection Geocode(string location, short results) => Geocode(location, results, LangType.ru_RU);

		public static GeoObjectCollection Geocode(string location, short results, LangType lang)
		{
			string request_ulr = $@"https://geocode-maps.yandex.ru/1.x/?geocode={location}&spn=0.5,0.4";

			return new GeoObjectCollection(DownloadString(request_ulr)); 
		}

		public static GeoObjectCollection Geocode(string location, short results, LangType lang, SearchArea search_area, bool rspn = false)
		{
			var request_ulr =
				Format(RequesrtUrl, StringMakeValid(location), results, LangTypeToStr(lang)) +
				$"&ll={search_area.LongLat.ToString("{0},{1}")}&spn={search_area.Spread.ToString("{0},{1}")}&rspn={(rspn ? 1 : 0)}" +
				(IsNullOrEmpty(Key) ? Empty : "&key=" + Key);

			return new GeoObjectCollection(DownloadString(request_ulr));
		}


		private static string ReGeocode(double _long, double _lat) => "";

		private static string StringMakeValid(string location)
			=> location.Replace(" ", "+").Replace("&", "").Replace("?", "");
		
		private static string LangTypeToStr(LangType lang)
		{
			switch (lang)
			{
				case LangType.ru_RU: return "ru-RU";
				case LangType.uk_UA: return "uk-UA";
				case LangType.be_BY: return "be-BY";
				case LangType.en_US: return "en-US";
				case LangType.en_BR: return "en-BR";
				case LangType.tr_TR: return "tr-TR";
				default: return "ru-RU";
			}
		}
		private static string DownloadString(string url)
		{
			WebRequest request = WebRequest.Create(url);
			request.Credentials = CredentialCache.DefaultCredentials;
			using (HttpWebResponse response = (HttpWebResponse)request.GetResponseAsync().Result)
			using (Stream dataStream = response.GetResponseStream())
			using (StreamReader reader = new StreamReader(dataStream))
				return reader.ReadToEnd();
		}
	}
}
