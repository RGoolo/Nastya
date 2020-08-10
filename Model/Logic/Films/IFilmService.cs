using System.Collections.Generic;
using System.Threading.Tasks;

namespace Model.Logic.Films
{
    public interface IFilmService
    {
        Task<Film> GetFilm(int id);
        Task<List<Film>> GetFilms(string name);
    }
}