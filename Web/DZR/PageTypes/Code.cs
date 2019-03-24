namespace Web.DZR
{
	public class Code
	{
		public string Name { get; }
		public string Answer { get; }
		public bool Accepted { get; }
		public int Count { get; }

		public Code(string name, bool accepted, int count)
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
			Count = count;
		}

		public override string ToString()
		{
			if (Accepted)
				return $"{Count}✅{Name}" + (Answer == null ? string.Empty : $"({Answer})");
			else
				return $"{Count}❌{Name}";
		}
	}
}
