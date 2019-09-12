using System;
using System.Collections.Generic;
using System.Linq;

namespace Model.Logic.Braille
{
    public static class AlphaBetWorker
    {
        private static readonly List<AlphaBet> AlphaBet;

        static AlphaBetWorker()
        {
            AlphaBet = new List<AlphaBet>() { new AlphaBetRu(), new AlphaBetEn(), new AlphaBetDigital()};
        }

        public static BrailleSymbols[] GetTranslate(string[] str) => AlphaBet.Select(x => x.GetTranslate(str)).ToArray();

        public static BrailleSymbol GetBrailleSymbol(char str) => AlphaBet.Select(x => x.GetTranslate(str)).FirstOrDefault(x => x != null);

        public static char[] GetBraille(string[] str) => str.Select(x => (char)(BrailleSymbol.GetMask(x) + '⠀')).ToArray();

        public static AlphaBet GetAlphabet(string name) =>
            AlphaBet.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
    }
}