using System;
using System.IO;

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
		private readonly StreamWriter _file;
		public static string FileLog { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NightGameBot", "Log",$"log_{DateTime.Now}.txt");

		private static StreamWriter _staticFile;

		public static ILogger CreateLogger(string className)
		{
			if (_staticFile == null)
			{
				var path = Path.GetDirectoryName(FileLog);
				if (!Directory.Exists(path))
					Directory.CreateDirectory(path);

				_staticFile = new StreamWriter(FileLog, true);
			}

			return new Logger(className, _staticFile);
		}

		private Logger(string className, StreamWriter file)
		{
			_className = className;
			_file = file;
		}

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
			_file.WriteLine($"{_className} Info:{message}");
		}

		public void Info(string format, params object[] formatArgs)
		{
			throw new NotImplementedException();
		}

		public void Warning(string message) => _file.WriteLine($"{_className} warning:{message}");
		public void Warning(string message, params object[] formatArgs) => _file.WriteLine($"{_className} warning:{string.Format(message, formatArgs)}");
		public void Warning(Exception exception) => _file.WriteLine($"{_className} warning:{exception.Message}\n{exception.StackTrace}");
		public void Warning(Exception exception, string message) => _file.WriteLine($"{_className} warning:{message}\n{exception.Message}\n{exception.StackTrace}");

		public void Error(string message) => _file.WriteLine($"{_className} error:{message}");
		public void Error(string message, params object[] formatArgs) => _file.WriteLine($"{_className} error:{string.Format(message, formatArgs)}");
		public void Error(Exception exception) => _file.WriteLine($"{_className} error:{exception.Message}\n{exception.StackTrace}");
		public void Error(Exception exception, string message) => _file.WriteLine($"{_className} error:{message}\n{exception.Message}\n{exception.StackTrace}");
	}
}
