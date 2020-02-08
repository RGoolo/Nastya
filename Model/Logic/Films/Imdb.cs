using System.Threading.Tasks;
using Model.HttpMessages;

namespace Model.Logic.Films
{
	public class Imdb
	{
		private IHttpMessages _messeges;

		public Imdb()
		{
			_messeges = HttpMessagesFactory.GetMessages();
		}

		public async Task<Film> GetFilm(string name)
		{
			name = name.Replace(" ", "+");

			var document = await _messeges.GetDocumentNode($"https://www.kinopoisk.ru/index.php?kp_query={name}");

			var contentBlock = document.SelectSingleNode("//div[@class='element most_wanted']");
			var nameBlock = contentBlock.SelectSingleNode("//p[@class='name']");
		//	var yearsBlock = nameBlock.SelectSingleNode("//span[@class='year']");
			var linkBlock = nameBlock.FirstChild;

			var id = linkBlock.GetAttributeValue("data-id", 0);
			return new Film(id, nameBlock.InnerHtml, $"https://st.kp.yandex.net/images/film_big/{id}.jpg");
		}

	}
}