using System.Linq;
using System.Threading.Tasks;
using Model.BotTypes.Attribute;
using Model.BotTypes.Class;
using Model.BotTypes.Enums;
using Model.Logic.Braille;
using Model.Logic.Films;
using Model.Logic.Settings;

namespace Nastya.Commands
{
	
		[CommandClass(nameof(Film), "Брайль. Порядок ввода:\n1\t4\n2\t5\n3\t6\n", TypeUser.User)]
		public class Film
		{
			private readonly ISettings _settings;
			private Kinopoisk _poisk;

			public Film(ISettings settings)
			{
				_settings = settings;
				_poisk = new Kinopoisk();
			}

			//ToDo:
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