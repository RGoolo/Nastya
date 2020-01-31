using System;

namespace Model.Logger
{
	public interface ILogger : Grpc.Core.Logging.ILogger
	{
		void Error(Exception exception);
		void Warning(Exception exception);
	}

	public class Logger : ILogger
	{
		private readonly string _className;

		public static ILogger CreateLogger(string className)
		{
			return new Logger(className);
		}

		private Logger(string className) =>	_className = className;

		public Grpc.Core.Logging.ILogger ForType<T>()
		{
			throw new NotImplementedException();
		}

		public void Debug(string message)
		{
			throw new NotImplementedException();
		}

		public void Debug(string format, params object[] formatArgs)
		{
			throw new NotImplementedException();
		}

		public void Info(string message)
		{
			Console.WriteLine($"{_className} Info:{message}");
		}

		public void Info(string format, params object[] formatArgs)
		{
			throw new NotImplementedException();
		}

		public void Warning(string message) => Console.WriteLine($"{_className} warning:{message}");
		public void Warning(string message, params object[] formatArgs) => Console.WriteLine($"{_className} warning:{string.Format(message, formatArgs)}");
		public void Warning(Exception exception) => Console.WriteLine($"{_className} warning:{exception.Message}\n{exception.StackTrace}");
		public void Warning(Exception exception, string message) => Console.WriteLine($"{_className} warning:{message}\n{exception.Message}\n{exception.StackTrace}");

		public void Error(string message) => Console.WriteLine($"{_className} error:{message}");
		public void Error(string message, params object[] formatArgs) => Console.WriteLine($"{_className} error:{string.Format(message, formatArgs)}");
		public void Error(Exception exception) => Console.WriteLine($"{_className} error:{exception.Message}\n{exception.StackTrace}");
		public void Error(Exception exception, string message) => Console.WriteLine($"{_className} error:{message}\n{exception.Message}\n{exception.StackTrace}");
	}
}
