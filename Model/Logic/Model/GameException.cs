using System;

namespace Model.Logic.Model
{
	public class ModelException : Exception
	{
		public ModelException() { }
		public ModelException(string s) : base(s) { }
	}

	public class GameException : ModelException
	{
		public GameException() { }
		public GameException(string s) : base(s) { }
	}
}