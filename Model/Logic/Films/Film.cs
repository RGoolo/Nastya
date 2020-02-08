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
	}
}