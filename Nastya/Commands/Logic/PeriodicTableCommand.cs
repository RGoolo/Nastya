using System.Linq;
using Model.Bots.BotTypes.Attribute;
using Model.Bots.BotTypes.Enums;
using Model.Logic.PereodicTable;

namespace Nastya.Commands
{
	[CommandClass("PTable", "Таблица менделлева:", TypeUser.User)]
	public class PeriodicTableCommand	 
	{
		private readonly PeriodicTable _pereodicTable = new PeriodicTable();

		[Command(nameof(Men), "Переводит строку в обозначения елементов.", TypeUser.User)]
		public string Men(string[] elems) => string.Join(" ", _pereodicTable.GetElements(elems).Select(x => x.Equals(default(Element)) ? "-" : x.Symbol.ToString()));

		[Command(nameof(MenAtom), "Переводит строку в атомную масу елементов.", TypeUser.User)]
		public string MenAtom(string[] elems) => string.Join(" ", _pereodicTable.GetElements(elems).Select(x => x.Equals(default(Element)) ? "-" : x.AtomicNumber.ToString()));

		[Command(nameof(Mens), "Вывести элементы таблицы.",  TypeUser.User)]
		public string Mens(string[] elems) => string.Join("\n\n", _pereodicTable.GetElements(elems).Select(x => x.Equals(default(Element)) ? "-" : x.ToString()));
	}
}
