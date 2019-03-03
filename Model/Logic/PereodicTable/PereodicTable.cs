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
		public string AtomicMass { get; set; }
		public string Symbol { get; set; }
		public string RuName { get; set; }
		public string Name { get; set; }
		public string ElectronicConfiguration { get; set; }
		public override string ToString() => $"{AtomicNumber}\t{Symbol}\t{ElectronicConfiguration}" +
			$"\n{RuName}\n{Name}\n{AtomicMass}"; 
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
				var strElems = strElem.Split(',');
				var elem = new Element();
				{
					elem.AtomicNumber = int.Parse(strElems[0]);
				elem.Symbol = strElems[1].Trim();
				elem.Name = strElems[2].Trim();
				elem.AtomicMass = strElems[3];
				elem.ElectronicConfiguration = strElems[4];
				elem.RuName = "Водород";
				}
				result.Add(elem);
			
			}
			return result;
		}

		private static class VeryLazyfile
		{
			public static string elements = @"atomicNumber, symbol, name, atomicMass, electronicConfiguration, standardState
1, H , Hydrogen, 1.00794(4),1s1 ,gas
2, He , Helium, 4.002602(2),1s2 ,gas
3, Li , Lithium, 6.941(2),[He] 2s1 ,solid
4, Be , Beryllium, 9.012182(3),[He] 2s2 ,solid
5, B , Boron, 10.811(7),[He] 2s2 2p1 ,solid
6, C , Carbon, 12.0107(8),[He] 2s2 2p2 ,solid
7, N , Nitrogen, 14.0067(2),[He] 2s2 2p3 ,gas
8, O , Oxygen, 15.9994(3),[He] 2s2 2p4 ,gas
9, F , Fluorine, 18.9984032(5),[He] 2s2 2p5 ,gas
10, Ne , Neon, 20.1797(6),[He] 2s2 2p6 ,gas
11, Na , Sodium, 22.98976928(2),[Ne] 3s1 ,solid
12, Mg , Magnesium, 24.3050(6),[Ne] 3s2 ,solid
13, Al , Aluminum, 26.9815386(8),[Ne] 3s2 3p1 ,solid
14, Si , Silicon, 28.0855(3),[Ne] 3s2 3p2 ,solid
15, P , Phosphorus, 30.973762(2),[Ne] 3s2 3p3 ,solid
16, S , Sulfur, 32.065(5),[Ne] 3s2 3p4 ,solid
17, Cl , Chlorine, 35.453(2),[Ne] 3s2 3p5 ,gas
18, Ar , Argon, 39.948(1),[Ne] 3s2 3p6 ,gas
19, K , Potassium, 39.0983(1),[Ar] 4s1 ,solid
20, Ca , Calcium, 40.078(4),[Ar] 4s2 ,solid
21, Sc , Scandium, 44.955912(6),[Ar] 3d1 4s2 ,solid
22, Ti , Titanium, 47.867(1),[Ar] 3d2 4s2 ,solid
23, V , Vanadium, 50.9415(1),[Ar] 3d3 4s2 ,solid
24, Cr , Chromium, 51.9961(6),[Ar] 3d5 4s1 ,solid
25, Mn , Manganese, 54.938045(5),[Ar] 3d5 4s2 ,solid
26, Fe , Iron, 55.845(2),[Ar] 3d6 4s2 ,solid
27, Co , Cobalt, 58.933195(5),[Ar] 3d7 4s2 ,solid
28, Ni , Nickel, 58.6934(4),[Ar] 3d8 4s2 ,solid
29, Cu , Copper, 63.546(3),[Ar] 3d10 4s1 ,solid
30, Zn , Zinc, 65.38(2),[Ar] 3d10 4s2 ,solid
31, Ga , Gallium, 69.723(1),[Ar] 3d10 4s2 4p1 ,solid
32, Ge , Germanium, 72.64(1),[Ar] 3d10 4s2 4p2 ,solid
33, As , Arsenic, 74.92160(2),[Ar] 3d10 4s2 4p3 ,solid
34, Se , Selenium, 78.96(3),[Ar] 3d10 4s2 4p4 ,solid
35, Br , Bromine, 79.904(1),[Ar] 3d10 4s2 4p5 ,liquid
36, Kr , Krypton, 83.798(2),[Ar] 3d10 4s2 4p6 ,gas
37, Rb , Rubidium, 85.4678(3),[Kr] 5s1 ,solid
38, Sr , Strontium, 87.62(1),[Kr] 5s2 ,solid
39, Y , Yttrium, 88.90585(2),[Kr] 4d1 5s2 ,solid
40, Zr , Zirconium, 91.224(2),[Kr] 4d2 5s2 ,solid
41, Nb , Niobium, 92.90638(2),[Kr] 4d4 5s1 ,solid
42, Mo , Molybdenum, 95.96(2),[Kr] 4d5 5s1 ,solid
43, Tc , Technetium, [98],[Kr] 4d5 5s2 ,solid
44, Ru , Ruthenium, 101.07(2),[Kr] 4d7 5s1 ,solid
45, Rh , Rhodium, 102.90550(2),[Kr] 4d8 5s1 ,solid
46, Pd , Palladium, 106.42(1),[Kr] 4d10 ,solid
47, Ag , Silver, 107.8682(2),[Kr] 4d10 5s1 ,solid
48, Cd , Cadmium, 112.411(8),[Kr] 4d10 5s2 ,solid
49, In , Indium, 114.818(3),[Kr] 4d10 5s2 5p1 ,solid
50, Sn , Tin, 118.710(7),[Kr] 4d10 5s2 5p2 ,solid
51, Sb , Antimony, 121.760(1),[Kr] 4d10 5s2 5p3 ,solid
52, Te , Tellurium, 127.60(3),[Kr] 4d10 5s2 5p4 ,solid
53, I , Iodine, 126.90447(3),[Kr] 4d10 5s2 5p5 ,solid
54, Xe , Xenon, 131.293(6),[Kr] 4d10 5s2 5p6 ,gas
55, Cs , Cesium, 132.9054519(2),[Xe] 6s1 ,solid
56, Ba , Barium, 137.327(7),[Xe] 6s2 ,solid
57, La , Lanthanum, 138.90547(7),[Xe] 5d1 6s2 ,solid
58, Ce , Cerium, 140.116(1),[Xe] 4f1 5d1 6s2 ,solid
59, Pr , Praseodymium, 140.90765(2),[Xe] 4f3 6s2 ,solid
60, Nd , Neodymium, 144.242(3),[Xe] 4f4 6s2 ,solid
61, Pm , Promethium, [145],[Xe] 4f5 6s2 ,solid
62, Sm , Samarium, 150.36(2),[Xe] 4f6 6s2 ,solid
63, Eu , Europium, 151.964(1),[Xe] 4f7 6s2 ,solid
64, Gd , Gadolinium, 157.25(3),[Xe] 4f7 5d1 6s2 ,solid
65, Tb , Terbium, 158.92535(2),[Xe] 4f9 6s2 ,solid
66, Dy , Dysprosium, 162.500(1),[Xe] 4f10 6s2 ,solid
67, Ho , Holmium, 164.93032(2),[Xe] 4f11 6s2 ,solid
68, Er , Erbium, 167.259(3),[Xe] 4f12 6s2 ,solid
69, Tm , Thulium, 168.93421(2),[Xe] 4f13 6s2 ,solid
70, Yb , Ytterbium, 173.054(5),[Xe] 4f14 6s2 ,solid
71, Lu , Lutetium, 174.9668(1),[Xe] 4f14 5d1 6s2 ,solid
72, Hf , Hafnium, 178.49(2),[Xe] 4f14 5d2 6s2 ,solid
73, Ta , Tantalum, 180.94788(2),[Xe] 4f14 5d3 6s2 ,solid
74, W , Tungsten, 183.84(1),[Xe] 4f14 5d4 6s2 ,solid
75, Re , Rhenium, 186.207(1),[Xe] 4f14 5d5 6s2 ,solid
76, Os , Osmium, 190.23(3),[Xe] 4f14 5d6 6s2 ,solid
77, Ir , Iridium, 192.217(3),[Xe] 4f14 5d7 6s2 ,solid
78, Pt , Platinum, 195.084(9),[Xe] 4f14 5d9 6s1 ,solid
79, Au , Gold, 196.966569(4),[Xe] 4f14 5d10 6s1 ,solid
80, Hg , Mercury, 200.59(2),[Xe] 4f14 5d10 6s2 ,liquid
81, Tl , Thallium, 204.3833(2),[Xe] 4f14 5d10 6s2 6p1 ,solid
82, Pb , Lead, 207.2(1),[Xe] 4f14 5d10 6s2 6p2 ,solid
83, Bi , Bismuth, 208.98040(1),[Xe] 4f14 5d10 6s2 6p3 ,solid
84, Po , Polonium, [209],[Xe] 4f14 5d10 6s2 6p4 ,solid
85, At , Astatine, [210],[Xe] 4f14 5d10 6s2 6p5 ,solid
86, Rn , Radon, [222],[Xe] 4f14 5d10 6s2 6p6 ,gas
87, Fr , Francium, [223],[Rn] 7s1 ,solid
88, Ra , Radium, [226],[Rn] 7s2 ,solid
89, Ac , Actinium, [227],[Rn] 6d1 7s2 ,solid
90, Th , Thorium, 232.03806(2),[Rn] 6d2 7s2 ,solid
91, Pa , Protactinium, 231.03588(2),[Rn] 5f2 6d1 7s2 ,solid
92, U , Uranium, 238.02891(3),[Rn] 5f3 6d1 7s2 ,solid
93, Np , Neptunium, [237],[Rn] 5f4 6d1 7s2 ,solid
94, Pu , Plutonium, [244],[Rn] 5f6 7s2 ,solid
95, Am , Americium, [243],[Rn] 5f7 7s2 ,solid
96, Cm , Curium, [247],[Rn] 5f7 6d1 7s2 ,solid
97, Bk , Berkelium, [247],[Rn] 5f9 7s2 ,solid
98, Cf , Californium, [251],[Rn] 5f10 7s2 ,solid
99, Es , Einsteinium, [252],[Rn] 5f11 7s2 ,solid
100, Fm , Fermium, [257],[Rn] 5f12 7s2 ,
101, Md , Mendelevium, [258],[Rn] 5f13 7s2 ,
102, No , Nobelium, [259],[Rn] 5f14 7s2 ,
103, Lr , Lawrencium, [262],[Rn] 5f14 7s2 7p1 ,
104, Rf , Rutherfordium, [267],[Rn] 5f14 6d2 7s2 ,
105, Db , Dubnium, [268],[Rn] 5f14 6d3 7s2 ,
106, Sg , Seaborgium, [271],[Rn] 5f14 6d4 7s2 ,
107, Bh , Bohrium, [272],[Rn] 5f14 6d5 7s2 ,
108, Hs , Hassium, [270],[Rn] 5f14 6d6 7s2 ,
109, Mt , Meitnerium, [276],[Rn] 5f14 6d7 7s2 ,
110, Ds , Darmstadtium, [281],[Rn] 5f14 6d9 7s1 ,
111, Rg , Roentgenium, [280],[Rn] 5f14 6d10 7s1 ,
112, Cn , Copernicium, [285],[Rn] 5f14 6d10 7s2 ,
113, Nh , Nihonium, [284],[Rn] 5f14 6d10 7s2 7p1 ,
114, Fl , Flerovium, [289],[Rn] 5f14 6d10 7s2 7p2 ,
115, Mc , Moscovium, [288],[Rn] 5f14 6d10 7s2 7p3 ,
116, Lv , Livermorium, [293],[Rn] 5f14 6d10 7s2 7p4 ,
117, Ts , Tennessine, [294],[Rn] 5f14 6d10 7s2 7p5 ,
118, Og , Oganesson, [294] ,[Rn] 5f14 6d10 7s2 7p6 ,";
		}
	}
}
