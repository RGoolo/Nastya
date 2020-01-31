using System;
using System.Collections.Generic;
using System.Linq;

namespace Model.Logic.PereodicTable
{
	public struct Element
	{
		public int Group { get; set; }
		public int Period { get; set; }
		public int AtomicNumber { get; set; }
		public double AtomicMass { get; set; }
		public string Symbol { get; set; }
		public string RuName { get; set; }
		public string Name { get; set; }
		public string ElectronicConfiguration { get; set; }
		public string State { get; set; }
		public override string ToString() => $"{AtomicNumber}\t{Symbol}\t{ElectronicConfiguration}" +
			$"\n{RuName}\n{Name}\n{AtomicMass}\n{State}"; 
	}

	public class PeriodicTable
	{
		private readonly Dictionary<int, Element> _elements = new Dictionary<int, Element>();

		public PeriodicTable(IEnumerable<Element> elements)
		{
			foreach (var element in elements)
			{
				_elements.Add(element.AtomicNumber, element);
			}
		}

		public PeriodicTable() : this(PeriodicTableSingleton.GetElements())
		{
		}

		public Element this[int atomicNumber] => _elements[atomicNumber];
		public List<Element> GetElements(string[] strs) => strs?.Select(GetElement).ToList() ?? new List<Element>();
		public List<Element> GetElements(string str) => GetElements(str.Split(' '));

		public Element GetElement(string str)
		{
			if (int.TryParse(str, out int atomicNumber))
				return this[atomicNumber];

			var elemProps = typeof(Element).GetProperties().Where(x => x.PropertyType == typeof(string)).ToList();
			return _elements.Values.FirstOrDefault(elem =>
				elemProps.Any(propInfo => string.Equals((string)propInfo.GetValue(elem), str, StringComparison.InvariantCultureIgnoreCase)));
		}
	}

	public class PeriodicTableSingleton
	{
		public static List<Element> GetElements()
		{
			var result = new List<Element>();
			foreach (var strElem in VeryLazyfile.elements.Split('\n').Skip(1))
			{
				var strElems = strElem.Split('\t');
				var i = 0;
				var elem = new Element();
				{
					elem.AtomicNumber = int.Parse(strElems[i++]);
					elem.Symbol = strElems[i++];
					elem.RuName = strElems[i++];
					elem.Name = strElems[i++];

					elem.AtomicMass =double.Parse(strElems[i++]);
					elem.ElectronicConfiguration = strElems[i++];
					elem.State = strElems[i++];
				}
				result.Add(elem);
			
			}
			return result;
		}

		private static class VeryLazyfile
		{
			public static string elements = @"atomicNumber	symbol	ru name	en name	atomicMass	 electronicConfiguration	standardState
1	H	Водород	Hydrogen	1,00794	1s1	gas
2	He	Гелий	Helium	4,002602	1s2	gas
3	Li	Литий	Lithium	6,9412	2s1	solid
4	Be	Бериллий	Beryllium	9,0122	2s2	solid
5	B	Бор	Boron	10,812	2s22p1	solid
6	С	Углерод	Carbon	12,011	2s22p2	solid
7	N	Азот	Nitrogen	14,0067	2s22p3	gas
8	О	Кислород	Oxygen	15,9994	2s22p4	gas
9	F	Фтор	Fluorine	18,9984	2s22p5	gas
10	Ne	Неон	Neon	20,179	2s22p6	gas
11	Na	Натрий	Sodium	22,98977	3s1	solid
12	Mg	Магний	Magnesium	24,305	3s2	solid
13	Al	Алюминий	Aluminum	26,98154	3s23p1	solid
14	Si	Кремний	Silicon	28,086	3s23p2	solid
15	P	Фосфор	Phosphorus	30,97376	3s23p3	solid
16	S	Сера	Sulfur	32,06	3s23p4	solid
17	Cl	Хлор	Chlorine	35,453	3s23p5	gas
18	Ar	Аргон	Argon	39,948	3s23p6	gas
19	К	Калий	Potassium	39,0983	4s1	solid
20	Ca	Кальций	Calcium	40,08	4s2	solid
21	Sc	Скандий	Scandium	44,9559	3d14s2	solid
22	Ti	Титан	Titanium	47,9	3d24s2	solid
23	V	Ванадий	Vanadium	50,9415	3d34s2	solid
24	Cr	Хром	Chromium	51,996	3d54s1	solid
25	Mn	Марганец	Manganese	54,938	3d54s2	solid
26	Fe	Железо	Iron	55,847	3d64s2	solid
27	Со	Кобальт	Cobalt	58,9332	3d74s2	solid
28	Ni	Никель	Nickel	58,7	3d84s2	solid
29	Cu	Медь	Copper	63,546	3d104s1	solid
30	Zn	Цинк	Zinc	65,38	3d104s2	solid
31	Ga	Галлий	Gallium	69,72	3d104s24p1	solid
32	Ge	Германий	Germanium	72,59	3d104s24p2	solid
33	As	Мышьяк	Arsenic	74,9216	3d104s24p3	solid
34	Se	Селен	Selenium	78,96	3d104s24p4	solid
35	Br	Бром	Bromine	79,904	3d104s24p5	liquid
36	Kr	Криптон	Krypton	83,8	3d104s24p6	gas
37	Rb	Рубидий	Rubidium	85,4678	5s1	solid
38	Sr	Стронций	Strontium	87,62	5s2	solid
39	Y	Иттрий	Yttrium	88,9059	4d15s2	solid
40	Zr	Цирконий	Zirconium	91,2	4d25s2	solid
41	Nb	Ниобий	Niobium	92,9064	4d45s1	solid
42	Mo	Молибден	Molybdenum	95,94	4d55s1	solid
43	Tc	Технеций	Technetium	98,9062	4d55s2	solid
44	Ru	Рутений	Ruthenium	101,07	4d75s1	solid
45	Rh	Родий	Rhodium	102,9055	4d85s1	solid
46	Pd	Палладий	Palladium	106,4	4d10	solid
47	Ag	Серебро	Silver	107,868	4d105s1	solid
48	Cd	Кадмий	Cadmium	112,41	4d105s2	solid
49	In	Индий	Indium	114,82	4d105s25p1	solid
50	Sn	Олово	Tin	118,69	4d105s25p2	solid
51	Sb	Сурьма	Antimony	121,75	4d105s25p3	solid
52	Те	Теллур	Tellurium	127,6	4d105s25p4	solid
53	I	Йод	Iodine	126,9045	4d105s25p5	solid
54	Xe	Ксенон	Xenon	131,3	4d105s25p6	gas
55	Cs	Цезий	Cesium	132,9054	6s1	solid
56	Ba	Барий	Barium	137,33	6s2	solid
57	La	Лантан	Lanthanum	138,9	5d16s2	solid
58	Ce	Церий	Cerium	140,12	4f15d16s2	solid
59	Pr	Празеодим	Praseodymium	140,9	4f36s2	solid
60	Nd	Неодим	Neodymium	144,24	4f46s2	solid
61	Pm	Прометий	Promethium	145	4f56s2	solid
62	Sm	Самарий	Samarium	150,35	4f66s2	solid
63	Eu	Европий	Europium	151,96	4f76s2	solid
64	Gd	Гадолиний	Gadolinium	157,25	4f75d16s2	solid
65	Tb	Тербий	Terbium	158,92	4f96s2	solid
66	Dy	Диспрозий	Dysprosium	162,5	4f106s2	solid
67	Ho	Гольмий	Holmium	164,93	4f116s2	solid
68	Er	Эрбий	Erbium	167,26	4f126s2	solid
69	Tm	Тулий	Thulium	168,93	4f136s2	solid
70	Yb	Иттербий	Ytterbium	173,04	4f146s2	solid
71	Lu	Лютеций	Lutetium	174,97	4f145d16s2	solid
72	Hf	Гафний	Hafnium	178,49	4f145d26s2	solid
73	Ta	Тантал	Tantalum	180,9479	4f145d36s2	solid
74	W	Вольфрам	Tungsten	183,85	4f145d46s2	solid
75	Re	Рений	Rhenium	186,207	4f145d56s2	solid
76	Os	Осмий	Osmium	190,2	4f145d66s2	solid
77	Ir	Иридий	Iridium	192,22	4f145d76s2	solid
78	Pt	Платина	Platinum	195,09	4f145d96s1	solid
79	Au	Золото	Gold	196,9665	4f145d106s1	solid
80	Hg	Ртуть	Mercury	200,59	4f145d106s2	liquid
81	Tl	Таллий	Thallium	204,37	4f145d106s26p1	solid
82	Pb	Свинец	Lead	207,2	4f145d106s26p2	solid
83	Bi	Висмут	Bismuth	208,9	4f145d106s26p3	solid
84	Po	Полоний	Polonium	209	4f145d106s26p4	solid
85	At	Астат	Astatine	210	4f145d106s26p5	solid
86	Rn	Радон	Radon	222	4f145d106s26p6	gas
87	Fr	Франций	Francium	223	7s1	solid
88	Ra	Радий	Radium	226	7s2	solid
89	Ac	Актиний	Actinium	227	6d17s2	solid
90	Th	Торий	Thorium	232,03	6d27s2	solid
91	Pa	Протактиний	Protactinium	231,03	5f26d17s2	solid
92	U	Уран	Uranium	238,02	5f36d17s2	solid
93	Np	Нептуний	Neptunium	237,04	5f46d17s2	solid
94	Pu	Плутоний	Plutonium	244,06	5f67s2	solid
95	Am	Америций	Americium	243,06	5f77s2	solid
96	Cm	Кюрий	Curium	247,07	5f76d17s2	solid
97	Bk	Берклий	Berkelium	247,07	5f97s2	solid
98	Cf	Калифорний	Californium	251,07	5f107s2	solid
99	Es	Эйнштейний	Einsteinium	252,08	5f117s2	solid
100	Fm	Фермий	Fermium	257,08	5f127s2	-
101	Md	Менделевий	Mendelevium	258,09	5f137s2	-
102	No	Нобелий	Nobelium	259,1	5f147s2	-
103	Lr	Лоуренсий	Lawrencium	260,1	5f147s27p1	-
104	Rf	Резерфордий	Rutherfordium	261	5f146d27s2	-
105	Db	Дубний	Dubnium	262	5f146d37s2	-
106	Sg	Сиборгий	Seaborgium	266	5f146d47s2	-
107	Bh	Борий	Bohrium	267	5f146d57s2	-
108	Hs	Хассий	Hassium	269	5f146d67s2	-
109	Mt	Мейтнерий	Meitnerium	276	5f146d77s2	-
110	Ds	Дармштадтий	Darmstadtium	227	5f146d97s1	-
111	Rg	Ренгений	Roentgenium	280	5f146d107s1	-
112	Cn	Коперниций	Copernicium	285	5f146d107s2	-
113	Uut	Унунтрий	Nihonium	284	5f146d107s27p1	-
114	Uuq	Унунквадий	Flerovium	289	5f146d107s27p2	-
115	Uup	Унунпентий	Moscovium	288	5f146d107s27p3	-
116	Uuh	Унунгексий	Livermorium	293	5f146d107s27p4	-
117	Uus	Унунсептий	Tennessine	294	5f146d107s27p5	-
118	Uuo	Унуноктий	Oganesson	294	5f146d107s27p6	-
119	Uuе	Унуненний	-	316	-	-
120	Ubn	Унбинилий	-	320	-	-
121	Ubu	Унбиуний	-	320	-	-
122	Ubb	Унбибий	-	0	-	-
123	Ubt	Унбитрий	-	0	-	-
124	Ubq	Унбиквадий	-	0	-	-
125	Ubp	Унбипентий	-	332	-	-
126	Ubn	Унбигексий	-	322	-	-";
		}
	}
}
