using System.Collections.Generic;

namespace Model.Logic.Films
{
	public class Film
	{
		public Film(int id, string name, string pic)
		{
			Pic = pic;
			Id = id;
			Name = name;
		}

		public string Pic { get; }
		public int Id { get; }
		public string Name { get; }

		public List<FilmProperty> Properties { get; } = new List<FilmProperty>();
	}

    public class FilmProperty
    {
        public FilmProperty(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get;  }
        public string Value { get; }
    }
}