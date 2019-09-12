using System.Collections.Generic;
using System.Linq;

namespace Model.Logic.Braille
{
    public abstract class AlphaBet
    {
        protected Dictionary<sbyte, BrailleSymbol> PrivateAlphaBet { get; set; }
        public abstract string Name { get; }
        protected abstract string LazyFileBraille { get; }
        public List<BrailleSymbol> SortAlphaBet;
        protected AlphaBet()
        {
            SortAlphaBet = LazyFileBraille.Split("\n").Select(x => new BrailleSymbol(x)).ToList();
            PrivateAlphaBet = SortAlphaBet.ToDictionary(x => x.Mask, x => x);
        }


        public BrailleSymbols GetTranslate(string[] str) => new BrailleSymbols(Name, GetTranslateSymbols(str));

        private BrailleSymbol[] GetTranslateSymbols(string[] str) => 
            str.Select(BrailleSymbol.GetMask)
                .Select(x => PrivateAlphaBet.ContainsKey(x) ? PrivateAlphaBet[x] : new BrailleSymbol(x)).ToArray();

        public BrailleSymbol GetTranslate(char str)
            => PrivateAlphaBet.Values.FirstOrDefault(x => x.Contain(str));
    }
}