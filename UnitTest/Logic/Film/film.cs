using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model.Logic.Braille;
using Model.Logic.Films;
using Xunit;

namespace UnitTest.Logic.Film
{
    public class film
    {
        [Fact]
        public async void CheckDigital()
        {
            var imdb = new Imdb();
            var film = await imdb.GetFilm(0109506);

            var imdb1 = new Imdb();
            var film1 = await imdb.GetFilms("ворон");
        }
    }
}
