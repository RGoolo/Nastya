using System.Linq;
using System.Text;

namespace Model.Logic.Braille
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
            Mask = MaskFromBraille(Symbol);
            AbSymbols = new[] {syms[1][0], syms[1].ToLower()[0]};
        }

        public BrailleSymbol(sbyte mask)
        {
            Mask = mask;
            Symbol = (char) ('⠀' + mask);
            AbSymbols = new [] {' ',};
        }

        public sbyte MaskFromBraille(char braille) => (sbyte) (braille - '⠀');

        public static sbyte GetMask(string str)
        {
            sbyte mask = 0;
            foreach (var dig in str)
            {
                if (dig < '1' || dig > '8')
                    continue;

                mask += (sbyte)(1 << (dig - '0' - 1));
            }
            return mask;
        }

        public string GetMask()
        {
            var mask = new StringBuilder();
            for(var i = 0; i < 8; ++i)
                mask.Append((Mask >> i & 1) == 0 ? string.Empty : (i + 1).ToString());
            return mask.ToString();
        }

        public override string ToString() => $"{AbSymbols[0]}{Symbol}";

        public bool Contain(char chr) => AbSymbols.Any(x => x == chr);
    }
}