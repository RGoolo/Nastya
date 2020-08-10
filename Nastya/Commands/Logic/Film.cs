using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Model.Bots.BotTypes.Attribute;
using Model.Bots.BotTypes.Class;
using Model.Bots.BotTypes.Enums;
using Model.Bots.BotTypes.Interfaces.Ids;
using Model.Bots.BotTypes.Interfaces.Messages;
using Model.Logic.Films;
using Model.Logic.Settings;

namespace Nastya.Commands
{
	
		[CommandClass(nameof(Film), "Фильмы.", TypeUser.User)]
		public class Film
		{
			private readonly ISettings _settings;
			private readonly IFilmService _poisk;
			private readonly IFilmService _imdb;

			public Film(ISettings settings)
			{
				_settings = settings;
				_poisk = new Kinopoisk();
                _imdb = new Imdb();
			}

			[Command(Const.Film.NameKinopoisk, "id фильма или информацию по фильму по Id")]
			public Task<IList<IMessageToBot>> Kp(string[] msg, IMessageId msgId)
            {
                return GetFilm(msg, msgId, _poisk, Const.Film.NameKinopoisk);
            }

            [Command(nameof(Imdb), "id фильма или информацию по фильму по Id")]
            public Task<IList<IMessageToBot>> Imdb(string[] msg, IMessageId msgId)
            {
                return GetFilm(msg, msgId, _imdb, nameof(Imdb));
            }

            [Command(nameof(GetFilms), "Список фильмов по названию, не больше 10")]
            public Task<List<IMessageToBot>> Kps(string[] msg, IMessageId msgId)
            {
                return GetFilms(msg, msgId, _poisk, Const.Film.NameKinopoisk);
            }

            [Command(nameof(Imdbs), "Список фильмов по названию, не больше 10")]
            public Task<List<IMessageToBot>> Imdbs(string[] msg, IMessageId msgId)
            {
                return GetFilms(msg, msgId, _imdb, nameof(Imdb));
            }

            private async Task<IList<IMessageToBot>> GetFilm(string[] msg, IMessageId msgId, IFilmService service, string nameFunction)
            {
                var result = new List<IMessageToBot>();
                var name = string.Join(" ", msg);
                var film = int.TryParse(name, out var id) ? await service.GetFilm(id) : (await service.GetFilms(name))?.FirstOrDefault();

                if (film == null)
                    result.Add(MessageToBot.GetTextMsg("Фильм не найден"));
                else if (film.Id == 0)
                    result.Add(MessageToBot.GetTextMsg(film.Name));
                else
                    result.AddRange(ToMsgs(film, msgId, nameFunction));

                return result;
            }

            private IEnumerable<IMessageToBot> ToMsgs(Model.Logic.Films.Film film, IMessageId msgId, string props, int countBatch = 10)
            {
                var text = new Texter(film.Name + "\t" + "\n/" + props + "_" + film.Id, true);
                var msg = MessageToBot.GetPhototMsg(_settings.FileChatFactory.InternetFile(film.Pic), text);
                msg.OnIdMessage = msgId;
                yield return msg;
                
                var list = new List<FilmProperty>();
                foreach (var filmProperty in film.Properties)
                {
                    list.Add(filmProperty);
                    if (list.Count != countBatch)
                        continue;
                    
                    yield return ToMsg(film, list, msgId);
                    list.Clear();
                }
                
                if (list.Count != 0)
                    yield return ToMsg(film, list, msgId);
            }
            
            private static IMessageToBot ToMsg(Model.Logic.Films.Film film, List<FilmProperty> props, IMessageId msgId)
            {
                var stingProps = props.Select(p => $"{p.Name}: {p.Value}".Replace("&nbsp;", " "));
                var text = new Texter(
                    film.Name
                    + System.Environment.NewLine
                    + string.Join(System.Environment.NewLine, stingProps)
                    , true, false);

                var msg = MessageToBot.GetTextMsg(text);
                msg.OnIdMessage = msgId;
                return msg;
            }

            private async Task<List<IMessageToBot>> GetFilms(string[] msg, IMessageId msgId, IFilmService service, string nameFunc)
            {
                var name = string.Join(" ", msg);
                var result = new List<IMessageToBot>();

                var films = await service.GetFilms(name);
                if (films == null || films.Count == 0)
                {
                    result.Add(MessageToBot.GetTextMsg("Фильм не найден"));
                    return result;
                }

                result.AddRange(films.Where(film => film != null).Take(10).SelectMany(film => ToMsgs(film, msgId, nameFunc)));
                return result;
            }
        }
	}