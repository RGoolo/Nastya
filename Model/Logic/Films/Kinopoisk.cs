using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Model.BotTypes.Class;
using Model.HttpMessages;
using Model.Logic.Settings;

namespace Model.Logic.Films
{
	public class Kinopoisk
	{
		private IHttpMessages _messeges;

		public Kinopoisk()
		{
			_messeges = HttpMessagesFactory.GetMessages();
		}

		public async Task<Film> GetFilm(string name)
		{
			name = name.Replace(" ", "+");
			
			var document = await _messeges.GetDocumentNode($"https://www.kinopoisk.ru/index.php?kp_query={name}");

			var contentBlock = document.SelectSingleNode("//div[@class='element most_wanted']");
			var nameBlock = contentBlock.SelectSingleNode("//p[@class='name']");
			var linkBlock = nameBlock.FirstChild;

			var id = linkBlock.GetAttributeValue("data-id", 0);
			return new Film(id, nameBlock.InnerHtml, $"https://st.kp.yandex.net/images/film_big/{id}.jpg");
		}

		public async Task<string> GetFilm(int filmId)
		{
			throw new Exception();
		}
	}
}