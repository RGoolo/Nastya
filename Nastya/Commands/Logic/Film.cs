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
			private readonly Kinopoisk _poisk;

			public Film(ISettings settings)
			{
				_settings = settings;
				_poisk = new Kinopoisk();
			}

			[Command(Const.Film.NameKinopoisk, "id фильма или информацию по фильму по Id")]
			public async Task<IList<IMessageToBot>> Kp(string[] msg, IMessageId msgId)
            {
                try
                {
                    var result = new List<IMessageToBot>();
                    var name = string.Join(" ", msg);
                    var film = int.TryParse(name, out var id) ? await _poisk.GetFilm(id) : await _poisk.GetFilm(name);

                    if (film == null)
                        result.Add(MessageToBot.GetTextMsg("Фильм не найден"));
                    else if (film.Id == 0)
                        result.Add(MessageToBot.GetTextMsg(film.Name));
                    else
                        result.AddRange(ToMsgs(film, msgId));

                    return result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                Console.WriteLine("succ");
            return null;
            }
            
            private IEnumerable<IMessageToBot> ToMsgs(Model.Logic.Films.Film film, IMessageId msgId, int countBatch = 10)
            {
                var text = new Texter(film.Name + "\t" + "\n/" + Const.Film.NameKinopoisk + "_" + film.Id, true);
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

            [Command(nameof(Kps), "Список фильмов по названию, не больше 10")]
            public async Task<List<IMessageToBot>> Kps(string[] msg, IMessageId msgId)
            {
                var name = string.Join(" ", msg);
			    var result = new List<IMessageToBot>();

                var films = await _poisk.GetFilms(name);
                if (films == null || films.Count == 0)
                {
                    result.Add(MessageToBot.GetTextMsg("Фильм не найден"));
                    return result;
                }

                result.AddRange(films.Where(film => film != null).Take(10).SelectMany(film => ToMsgs(film, msgId)));
                return result;
            }
	    }
	}