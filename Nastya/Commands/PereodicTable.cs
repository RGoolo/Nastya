using System.Linq;
using Model.Logic.PereodicTable;
using Model.Types.Attribute;

namespace Nastya.Commands
{
	[CommandClass(nameof(HelpCommand), "Таблица менделлева:", Model.Types.Enums.TypeUser.User)]
	public class PereodicTableCommand	 
	{
		private readonly PeriodicTable _pereodicTable = new PeriodicTable();

		[Command(nameof(Men), "Переводит строку в символы елементов.", Model.Types.Enums.TypeUser.User)]
		public string Men(string[] elems) => string.Join(" ", _pereodicTable.GetElements(elems).Select(x => x.Equals(default(Element)) ? "-" : x.Symbol.ToString()));

		[Command(nameof(MenDig), "Переводит строку в атомную масу елементов.", Model.Types.Enums.TypeUser.User)]
		public string MenDig(string[] elems) => string.Join(" ", _pereodicTable.GetElements(elems).Select(x => x.Equals(default(Element)) ? "-" : x.AtomicNumber.ToString()));

		[Command(nameof(Elements), "Полностью найденые элементы выводит.",  Model.Types.Enums.TypeUser.User)]
		public string Elements(string[] elems) => string.Join(" ", _pereodicTable.GetElements(elems).Select(x => x.Equals(default(Element)) ? "-" : x.ToString()));

		[Command(nameof(Element), "Выводит элемент таблицы.", Model.Types.Enums.TypeUser.User)]
		public string Element(string elem) => _pereodicTable.GetElement(elem).ToString();
	}
}
