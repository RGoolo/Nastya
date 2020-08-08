using System.Threading.Tasks;
using Model.Bots.BotTypes.Attribute;
using Model.Bots.BotTypes.Class;
using Model.Bots.BotTypes.Enums;
using Model.Logic.Films;
using Model.Logic.Settings;

namespace Nastya.Commands
{
	
		[CommandClass(nameof(Film), "Фильмы.", TypeUser.User)]
		public class Film
		{
			private readonly ISettings _settings;
			private Kinopoisk _poisk;

			public Film(ISettings settings)
			{
				_settings = settings;
				_poisk = new Kinopoisk();
			}

			[Command(Const.Film.NameKinopoisk, "Список фильмов или фильм по Id")]
			public async Task<IMessageToBot> Kp(string name)
			{
				if(int.TryParse(name, out var id))
				{
					//return _poisk.GetFilm(id);
				}

				var film = await _poisk.GetFilm(name);
				var test = new Texter(film.Name + "\t" + "\n/" + Const.Film.NameKinopoisk + "_" + film.Id, true);
				return MessageToBot.GetPhototMsg(_settings.FileChatFactory.InternetFile(film.Pic), test);
			}
		}
	}