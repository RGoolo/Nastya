using System;
using System.Collections.Generic;
using System.Linq;

namespace Model.Logic
{
	public class BrailleSymbol
	{
		public sbyte Mask { get; }
		public char Symbol;
		public char[] AbSymbols { get; set;}

		public BrailleSymbol(string str)
		{
			var syms = str.Split('\t');
			Symbol = syms[0][0];
			Mask = GetMask(syms[1]);
			AbSymbols = syms.Skip(2).Select(x => x.ToLower()[0]).ToArray();
		}

		public static sbyte GetMask(string str)
		{
			sbyte mask = 0;
			foreach (var dig in str)
			{
				if (dig < '1' || dig > '6')
					continue;

				mask += (sbyte)(1 >> dig - '0' - 1);
			}
			return mask;
		}

		public override string ToString() => Symbol + "(" + string.Join(",", AbSymbols) + ")";
	}

	public class Braille
	{
		private Dictionary<sbyte, BrailleSymbol> RuAlphaBet { get; }
		private Dictionary<sbyte, BrailleSymbol> EnAlphaBet { get; }

		public Braille()
		{
			Func<string, Dictionary<sbyte, BrailleSymbol>> toDict = (y) => y.Split("\n").Select(x => new BrailleSymbol(x)).ToDictionary(x => x.Mask, x => x);
			RuAlphaBet = toDict(LazyFileBrailleRu);
			EnAlphaBet = toDict(LazyFileBrailleEn);
		}

		public string GetTranslate(string[] strs) => "ru:" + GetTranslate(RuAlphaBet, strs) + "\n" + "en:" + GetTranslate(EnAlphaBet, strs);

		private string GetTranslate(Dictionary<sbyte, BrailleSymbol> alphaBet, string[] strs) 
			=> string.Join(" ", strs.Select(BrailleSymbol.GetMask).Select(x => alphaBet.ContainsKey(x) ? alphaBet[x].ToString() : "-").ToArray());

		private const string LazyFileBrailleRu = @"⠁	1	А	1
⠃	13	Б	2
⠺	2346	В
⠛	1245	Г	7
⠙	145	Д	4
⠑	15	Е	5
⠡	16	Ё
⠚	254	Ж	0
⠵	1563	З
⠊	24	И	9
⠯	41236	Й
⠅	13	К
⠇	123	Л
⠍	413	М
⠝	1453	Н
⠕	153	О
⠏	4123	П
⠗	1235	Р
⠎	423	С
⠞	4523	Т
⠥	136	У
⠋	412	Ф 6
⠓	125	Х 8
⠉	14	Ц 3
⠟	14253	Ч
⠱	156	Ш
⠭	1436	Щ
⠷	12536	Ъ
⠮	4236	Ы
⠾	42536	Ь
⠪	426	Э
⠳	1256	Ю
⠫	4126	Я";

		const string LazyFileBrailleEn = @"⠁	1	A	1
⠃	12	B	2
⠉	14	C	3
⠙	145	D	4
⠑	15	E	5
⠋	142	F	6
⠛	1425	G	7
⠓	125	H 8
⠊	24	I 9
⠚	254	J 0
⠅	13	K
⠇	123	L
⠍	143	M
⠝	1453	N
⠕	153	O
⠏	4123	P
⠟	41253	Q
⠗	1235	R
⠎	423	S
⠞	4523	T
⠥	136	U
⠧	1236	V
⠺	2456	W
⠭	1436	X
⠽	14536	Y
⠵	1563	Z
⠼	4563	№
⠲	256	.
⠂	2	,
⠢	26	?
⠆	23	;
⠤	36	-";
	}
}
