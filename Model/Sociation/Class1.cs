using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
// ToDo remove
namespace Model.Sociation
{
    public class ClassSoc
    {
	    readonly Dictionary<string, SortedSet<string>> _allSoc = new Dictionary<string, SortedSet<string>>();

		public ClassSoc()
		{
			FillDictionary();
		}

		public bool IsExist(string kubRai) => _allSoc.ContainsKey(kubRai);

		public IEnumerable<string> GetSoc(string kubRai)
		{
			if (IsExist(kubRai))
				return _allSoc[kubRai];
			else
				return new[] { "Нет ассоциаций" };
		}

		public IEnumerable<string> GetAnswer(string kubRai)
		{
			var AllWords = new List<SortedSet<String>>();
			var AllWords2 = new List<SortedSet<String>>();
			var result = new List<string>();

			kubRai.Split(' ').ToList().Where(IsExist).ToList().ForEach(x => AllWords.Add(_allSoc[x]));
			//kubRai.Split(' ').ToList().Where(x => IsExist(x)).ToList().ForEach(x => AllWords2.Add(allSoc[x]));

			for (int i = 0; i < AllWords.Count; i++)
			{
				//AllWords[i].ToList().ForEach(x => allSoc[x].ToList().ForEach(y => AllWords2[i].Add(y))) ;
				result = Concat(result, AllWords[i].ToList());
			}
			var abc = result.Where(x => _allSoc.ContainsKey(x));

			if (abc.Count() != 0)
				return abc;
			else
				return new[] { "Не решить" };
		}
		
		static List<string> Concat(List<string> a, List<string> b)
		{
			if (a.Count == 0) return b;
			var result = new List<string>();
			a.ForEach(x => b.ForEach(y => result.Add(x + y)));
			return result;
		}

		private void FillDictionary()
		{
			FillDictionaryByFile(@"D:\sociation\sociation1.org.tsv");
			FillDictionaryByFile(@"D:\sociation\mySoc.tsv");
		}
		private void FillDictionaryByFile(string path)
		{
			
			using (var fileStream = new FileStream(path, FileMode.Open))
			{
				using (var reader = new StreamReader(fileStream))
				{
					string line;
					while ((line = reader.ReadLine()) != null)
					{
						var line2 = line.Split('\t').ToList();

						if (!_allSoc.ContainsKey(line2[0]))
							_allSoc.Add(line2[0], new SortedSet<string>());
						_allSoc[line2[0]].Add(line2[1]);

						if (!_allSoc.ContainsKey(line2[1]))
							_allSoc.Add(line2[1], new SortedSet<string>());
						_allSoc[line2[1]].Add(line2[0]);
					}
				}
			}
		}
	}
}
