using System;

namespace Model
{
	public class Logger
	{
		private readonly string _className;

		public Logger (string className) =>	_className = className;

		public void WriteTrace(string msg) => Console.WriteLine($"{_className} trace:{msg}");

		public void WriteError(string msg) => Console.WriteLine($"{_className} error:{msg}");
	}
}
