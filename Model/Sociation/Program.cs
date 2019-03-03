using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Model.Sociation
{
	public class Program
	{
		const string path = @"D:\sociation\";
		const string MyWords = @"mySoc.tsv";
		const string Words = @"sociation.org.tsv";
		const string Words1 = @"sociation1.org.tsv";
		const string AnswerPath = @"D:\sociation\123.txt";

		public static void Main()
		{
			var soc = new ClassSoc();
			try
			{
				writeFile(AnswerPath, soc.GetAnswer("под г до д ъ и"));
				Console.WriteLine("finish");
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			Console.ReadLine();
		}

		private static void Start()
		{

			List<string> result = new List<string>();
			string word = "под г до д ъ и";
			{
				List<List<string>> myWords2 = new List<List<String>>();
				word.Split(' ').ToList().ForEach(x => myWords2.Add(getAllSocWords(x).ToList()));

				if (myWords2.Count > 0)
				{
					//MyWords[0].ForEach(x => result.Add(r)
					result = myWords2[0];

					for (int i = 1; i < myWords2.Count; i++)
					{
						result = Concat(result, myWords2[i]);
					}
				}
			}

			var res = getAllSortedSet();
			var answer = result.Where(x => res.Contains(x));

			writeFile(AnswerPath, answer);
		}

		static IEnumerable<string> CheckExistWord(List<string> word)
		{
			return word.GetRange(0,5);
		}

		static List<string> Concat(List<string> a, List<string> b)
		{
			var result = new List<string>();
			a.ForEach(x => b.ForEach(y => result.Add(x + y)));
			return result;
		}

		private static IEnumerable<string> getAllSocWords(string word)
		{
			var b = getSocWords(path + Words1, word).ToList();
			b.AddRange(getSocWords(path + MyWords, word).ToList());
			return b;
		}

		private static IEnumerable<string> getSocWords(string path, string word)
		{
			var result = new List<string>();

			using (var fileStream = new FileStream(path, FileMode.Open))
			{
				using (var reader = new StreamReader(fileStream))
				{
					string line;
					while ((line = reader.ReadLine()) != null)
					{
						//Console.WriteLine(line);
						var line2 = line.Split('\t').ToList();
						if (line2[0] == word)
							result.Add(line2[1]);
						if (line2[1] == word)
							result.Add(line2[0]);
					}
				}
			}
			return result;
		}

		private static SortedSet<string> getAllSortedSet()
		{
			var b = getSortedSet(path + Words1);
			b.Union(getSortedSet(path + MyWords));
			return b;
		}

		private static SortedSet<string> getSortedSet(string path)
		{
			var result = new SortedSet<string>();

			using (var fileStream = new FileStream(path, FileMode.Open))
			{
				using (var reader = new StreamReader(fileStream))
				{
					string line;
					while ((line = reader.ReadLine()) != null)
					{
						//Console.WriteLine(line);
						var line2 = line.Split('\t').ToList();
						result.Add(line2[1]);
						result.Add(line2[0]);
					}
				}
			}
			return result;
		}


		private static void readFile()
		{
			string word = "";


			FileStream fileStream = new FileStream(path + Words1, FileMode.Open);
			FileStream fileStream2 = new FileStream(path + MyWords, FileMode.Open);

			while (word.Length != 1)
			{
				string line;
				using (StreamReader reader = new StreamReader(fileStream))
				{
					while ((line = reader.ReadLine()) != null)
					{
						var a = line.Split('\t');
						if (a[0] == word)
							Console.WriteLine(a[1]);
						if (a[1] == word)
							Console.WriteLine(a[0]);
					}
				}
				using (StreamReader reader = new StreamReader(fileStream2))
				{
					while ((line = reader.ReadLine()) != null)
					{
						var a = line.Split('\t');
						if (a[0] == word)
							Console.WriteLine(a[1]);
						if (a[1] == word)
							Console.WriteLine(a[0]);
					}
				}
			}
		}

		private static void writeFile(string path, IEnumerable<string> str)
		{
			using (var logFile = File.Create(path))
			using (var logWriter = new StreamWriter(logFile))
				str.ToList().ForEach(x => logWriter.WriteLine(x));
		}
		private static void writeFile()
		{
			var fileStream = new FileStream(path + Words, FileMode.Open);
			var logFile = File.Create(path + Words1);

			using (var logWriter = new System.IO.StreamWriter(logFile))
			{
				using (var reader = new StreamReader(fileStream))
				{
					string line;
					while ((line = reader.ReadLine()) != null)
					{
						var line2 = line.Split('\t').ToList();
						logWriter.WriteLine(line2[0] + "\t" + line2[1]);
					}
				}
			}
			Console.WriteLine("finish");
			Console.ReadLine();
		}
	}
}