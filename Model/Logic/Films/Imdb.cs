using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BotModel.HttpMessages;
using HtmlAgilityPack;

namespace Model.Logic.Films
{
	public class Imdb : IFilmService
    {
		private IHttpMessages _messeges;

		public Imdb()
		{
			_messeges = HttpMessagesFactory.GetMessages();
		}

        private string GetId(int id)
        {
            var sId = id.ToString();
            return new string('0', 10 - sId.Length) + id;
        }

        public async Task<Film> GetFilm(int id)
        {
            var document = await _messeges.GetDocumentNode($"https://www.imdb.com/title/tt{GetId(id)}/");

            var contentBlock = document.SelectSingleNode("//div[@class='title_wrapper']");
            if (contentBlock == null)
                return null;

            var nameNode = Normalize(contentBlock.FirstChild);
            var name = nameNode.InnerText.Split("&nbsp;")[0];// .Replace("&nbsp", string.Empty);
            
            var yearNode = Normalize(Normalize(Normalize(nameNode.FirstChild).FirstChild));

            var postHrf = Normalize(Normalize(document.SelectSingleNode("//div[@class='poster']").FirstChild));
            var postImg = Normalize(postHrf.FirstChild).GetAttributeValue("src", string.Empty);

            var film = new Film(id, name, postImg);

            film.Properties.Add(new FilmProperty("год", yearNode.OuterHtml));
            film.Properties.AddRange(ToProperty(Normalize(nameNode.NextSibling.NextSibling.NextSibling)));
            return film;
        }

        //ToDo
        private List<FilmProperty> ToProperty(HtmlNode node)
        {
            var result = new List<FilmProperty>();
            foreach (var nodeChildNode in node.ChildNodes)
            {
                if (nodeChildNode.Name == "#text" || nodeChildNode.Name == "span")
                    continue;

                result.Add(new FilmProperty(nodeChildNode.Name, nodeChildNode.OuterHtml));
            }

            return result;
        }

        private int? IdFormUrl(string href)
        {
            if (!int.TryParse(href.Split('/', StringSplitOptions.RemoveEmptyEntries)[1].Substring(2), out var id))
                return null;
            return id;
        }

        public async Task<List<Film>> GetFilms(string name)
		{
			var result = new List<Film>();
            var document = await _messeges.GetDocumentNode($"https://www.imdb.com/find?q={name}&ref_=nv_sr_sm");

            var contentBlock = document.SelectSingleNode("//table[@class='findList']");
            if (contentBlock == null)
                return null;

            foreach (var node in contentBlock.ChildNodes)
            {
                var film = FromNode(node);
                if (film!= null)
                    result.Add(film);
            }

            return result;
        }

        private HtmlNode Normalize(HtmlNode node) =>  node.Name == "#text" ? node.NextSibling : node;

        private Film FromNode(HtmlNode node)
        {
            node = Normalize(Normalize(node));

            if (node.Name == "#text")
                return null;

            var photo = Normalize(node.FirstChild);

            if (photo.Name == "#text")
                photo = photo.NextSibling;

            var picture = Normalize(Normalize(photo.FirstChild).FirstChild).GetAttributeValue("src", null);

            var film = Normalize(Normalize(photo.NextSibling).FirstChild);
            var href = film.GetAttributeValue("href", null);
            var id = IdFormUrl(href);
            if (!id.HasValue)
                return null;

            return new Film(id.Value, film.InnerText, picture) ;
        }
    }
} 