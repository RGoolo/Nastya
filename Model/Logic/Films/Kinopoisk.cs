using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Model.Bots.BotTypes.Interfaces.Ids;
using Model.HttpMessages;
using Model.Logic.Settings;
using Model.Logic.Films;

namespace Model.Logic.Films
{
    public class Kinopoisk : IFilmService
    {
		private readonly IHttpMessages _messages;

		public Kinopoisk()
		{
			_messages = HttpMessagesFactory.GetMessages();
		}

        public async Task<Film> GetFilm(int id)
        {
            var url = $"https://www.kinopoisk.ru/film/{id}/";
            var document = await _messages.GetDocumentNode(url);

            if (document.InnerText.StartsWith("Ой!Нам"))
                throw new Exception(document.InnerText);
            
            var contentBlock = document.SelectSingleNode("//div[@class='styles_root__37rNk styles_basicInfoSection__3tXzP']");
            if (contentBlock == null)
                return null;
            
            var mainNode = contentBlock?.FirstChild;
            var nameNode = mainNode.FirstChild?.FirstChild?.NextSibling?.FirstChild?.FirstChild;
            if (nameNode == null)
                return null;

            var film = new Films.Film(id, $"<a href=\"https://www.kinopoisk.ru/film/{id}/\">{nameNode.InnerHtml}</a>", $"https://st.kp.yandex.net/images/film_big/{id}.jpg");

            var propsNode = mainNode.NextSibling?.FirstChild?.FirstChild?.NextSibling;
            if (propsNode == null)
                return null;

            foreach (var propNode in propsNode.ChildNodes)
            {
                var prop = ToProperty(propNode, $"https://www.kinopoisk.ru/film/{id}/");
                if (prop != null)
                    film.Properties.Add(prop);
            }

            return film;
        }

        private const string AHref = "href";
        
        private FilmProperty ToProperty(HtmlNode node, string url)
        {
            if (node == null)
                return null;

            var name = node.FirstChild.InnerHtml;
            var props = node.FirstChild.NextSibling;

            var firstProp = GetChildNode(props);
            if (firstProp == null)
                return null;

            if (firstProp.Name == "div" && firstProp.FirstChild.Name == "#text")
                return new FilmProperty(name, firstProp.InnerHtml);

            var list = new List<string>();
            foreach (var prop in props.ChildNodes)
            {
                if (prop.Name != "a")
                    continue;

                if(string.IsNullOrEmpty(AHref))
                    continue;

                list.Add($"<a href=\"{url }{prop.GetAttributeValue(AHref, string.Empty)}\">{firstProp.InnerText}</a>");
            }
            return new FilmProperty(name, String.Join(", ", list));
        }

        private HtmlNode GetChildNode(HtmlNode node)
        {
            if (node == null)
                return null;

            return node.GetAttributeValue("class", string.Empty) == "styles_valueDark__3dsUz styles_value__2F1uj"
                ? node.FirstChild.Name == "#text" || node.FirstChild.Name == "a"
                    ? node
                    : GetChildNode(node.FirstChild)
                : node;
        }

        private Film ToFilm(HtmlNode nodes)
        {
			var linkBlock = nodes.FirstChild;

            if (linkBlock == null)
                return null;

            var id = linkBlock.GetAttributeValue("data-id", 0);
            return new Films.Film(id, $"<a href=\"https://www.kinopoisk.ru/film/{id}/\">{nodes.InnerHtml}</a>", $"https://st.kp.yandex.net/images/film_big/{id}.jpg");
		}

        public async Task<List<Film>> GetFilms(string name)
        {
            name = name.Replace(" ", "+");
            var result = new List<Film>();
            var document = await _messages.GetDocumentNode($"https://www.kinopoisk.ru/index.php?kp_query={name}");

            var contentBlock = document.SelectSingleNode("//div[@class='element most_wanted']");
            if (contentBlock == null)
                return null;

            var nameBlocks = contentBlock.SelectNodes("//p[@class='name']");
            foreach (var block in nameBlocks)
            {
                if(block != null)
                    result.Add(ToFilm(block));
            }

            return result;
        }
    }
}