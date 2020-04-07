using Model.Bots.BotTypes.Class;

namespace Web.DZR
{
	public class Code
	{
		public string Name { get; }
		public string Answered { get; }
		public bool Accepted { get; }
		public int Count { get; }
		public Answer Answer { get; }

		public Code(string name, bool accepted, int count, Answer answer)
		{
			Name = name;

			if (name.Contains(":"))
			{
				var text = name.Split(":");
				Name = text[0];
				if (text.Length > 1)
					Answered = text[1];
			}
	
			Accepted = accepted;
			Count = count;
			Answer = answer;
		}

		public override string ToString() => ToString(false);
		

		public string ToString(bool useAnswer)
		{
			if (useAnswer && Answer != null)
				return $"{Count}✅{Name} прислал:" + (string.IsNullOrEmpty(Answer.User.Display) ? "-" : Answer.User.Display) + (Answer == null ? string.Empty : $" ({(Answer)})");

			return Accepted ? $"{Count}✅{Name}" + (Answer == null ? string.Empty : $"({(Answer)})") : $"{Count}❌{Name}";
		}
	}
}
