using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model.Types.Attribute;
using Model.Types.Interfaces;

namespace Nastya.Commands
{
	[CommandClass(nameof(SyntacticalAnalyzer), "Синтаксический анализ.", Model.Types.Enums.TypeUser.User)]
	public class SyntacticalAnalyzer
	{
		private const string replacesChars = "\n ";

		[Command(nameof(Anl), "Возращает текст лесенкой, первые и последние буквы.")]
		public string Anl(string[] str, IMessage msg)
		{
			if (msg.ReplyToMessage != null)
				str = msg.ReplyToMessage.Text.Split(replacesChars.ToArray(), StringSplitOptions.RemoveEmptyEntries).ToArray();

			var result = new StringBuilder();
			//var str = new string[] { "123", "456", "789", "zx" };
			result.Append("Первые: ");
			result.Append(str.Select(x => x.First()).ToArray());
			result.Append("\nПоследние: ");
			result.Append(str.Select(x => x.Last()).ToArray());
			var i = 0;
			result.Append("\nЛестницей: ");
			result.Append(str.Select(x => x.Length > i ? x[i++] : '-').ToArray());
			return result.ToString();
		}

		[Command(nameof(Alf), "Вернет либо алфавит, либо номера в алфавите.")]
		public string Alf(string[] strs, IMessage msg)
		{
			if (msg.ReplyToMessage != null)
				strs = msg.ReplyToMessage.Text.Split(replacesChars.ToArray(), StringSplitOptions.RemoveEmptyEntries).ToArray();

			return Alphabet.Alf(strs);
		}

		private class Alphabet
		{
			private static Dictionary<string, string> Alphabets = new Dictionary<string, string>()
				{
					{ "ru", "абвгдеёжзийклмнопрстуфхцчшщъыьэюя"},
					{ "en", "abcdefghijklmnopqrstuvwxyz"},
				//{"", "αΑβΒγΓδΔεΕζηΖΘθ Ηι κΚλ  μ ν  ξ ο π ρ σ τ υ φ χ ψ ω" } ΙΛΜΝΞΟΠΡΣΤΥΦΧΨΩ
				};

			public static string Alf(string[] strs)
			{
				if (!strs.Any())
					return null;
			
				if (int.TryParse(strs.First(), out var i))
					return Symbols(strs);
				else
					return Digitals(strs);
			}

			private static string Digitals(string[] strs)
			{
				var result = new StringBuilder();
				foreach (var alp in Alphabets.Values)
				{
					//Alphabets.Values
				}
				return result.ToString();
			}

			private static string Symbols(string[] strs)
			{
				var result = new StringBuilder();
				foreach (var alp in Alphabets)
				{
					result.Append(alp.Key + ":");
					foreach (var symb in strs)
					{
						if (int.TryParse(symb, out var integ) && integ < alp.Value.Length)
							result.Append(alp.Value[integ]);
						else
							result.Append("-");
					}
					result.Append(alp.Key + "\n");
				}
				return result.ToString();
			}
			/* easy
			public static char GetSymbols(int i)
			{
				i = i - 1;
				if (i > ('я' - 'а' + 1) || i < 0) return '-';
				if (i == 6) return 'ё';
				if (i > 6) return (char)('а' + i - 1);
				return (char)('а' + i);
			}

			public static char GetSymbolsEn(int i) => (i > ('z' - 'a' + 1) || i < 1) ? '-' : (char)('a' + i - 1);
			public static char GetSymbolsFa(int i) => (i > ('ω' - 'α' + 1) || i < 1) ? '-' : (char)('α' + i - 1);
			*/
		}
	}
}
