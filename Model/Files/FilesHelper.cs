using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Model.Files
{
	public class FileHelper
	{
		private readonly string _fullFileName;

		public static string GetNotExistFile(string directory, string ext, params string[] subFolders)
		{
			string file;
			do file = Path.Combine(directory, Path.Combine(subFolders), Guid.NewGuid() + ext);
			while (!File.Exists(file));
			return file;
		}

		public FileHelper(string fullFileName)
		{
			_fullFileName = fullFileName;
		}

		public void CreateFile()
		{
			File.Create(_fullFileName);
		}

		public void DeleteFile()
		{
			File.Delete(_fullFileName);
		}

		public static string ReadToEnd(string path)
		{
			using var sr = new StreamReader(path);
			return sr.ReadToEnd();
		}

		public static Task<string> ReadToEndAsync(string path)
		{
			var sr = new StreamReader(path);
			return sr.ReadToEndAsync();
		}

		public string ReadToEnd() => ReadToEnd(_fullFileName);

		public IEnumerable<string> ReadByLine()
		{
			using var sr = new StreamReader(_fullFileName);
			string line;
			while (null != (line = sr.ReadLine()))
				yield return line;
		}

		public void Write(string text)
		{
			using var sw = new StreamWriter(_fullFileName, false);
			sw.Write(text);
		}

		public void WriteToEnd(string text)
		{
			using var sw = new StreamWriter(_fullFileName, true);
			sw.Write(text);
		}
	}
}
