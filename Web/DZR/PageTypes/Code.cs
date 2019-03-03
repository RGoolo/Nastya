namespace Web.DZR
{
	public class Code
	{
		public string Name { get; }
		public string Answer { get; }
		public bool Accepted { get; }

		public Code(string name, bool accepted)
		{
			Name = name;

			if (name.Contains(":"))
			{
				var text = name.Split(":");
				Name = text[0];
				if (text.Length > 1)
					Answer = text[1];
			}
	
			Accepted = accepted;
		}

		public override string ToString()
		{
			if (Accepted)
				return $"{Name}" + (Answer == null ? string.Empty : $"({Answer})");
			else
				return $"<b>{Name}</b>;";
		}
	}
}
