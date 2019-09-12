using System.Linq;

namespace Model.Logic.Braille
{
    public class BrailleSymbols
    {
        private readonly string _name;
        public readonly BrailleSymbol[] Symbols;

        public BrailleSymbols(string name, BrailleSymbol[] symbols)
        {
            _name = name;
            Symbols = symbols;
        }

        public override string ToString() => _name + ":" + string.Join(" ", Symbols.Select(x => x.ToString()));
    }
}