using System.Collections.Generic;
using System.Linq;
using Model.Logic.Braille;
using Nastya.Commands;
using Xunit;

namespace UnitTest.Logic
{
    public class Braille
    {
        [Fact]
        public void CheckDigital()
        {
            var list = "0123456789";
            var symbols = list.Select(AlphaBetWorker.GetBrailleSymbol).ToArray();
            var masks = symbols.Select(x => x.GetMask()).ToArray();
            var result = string.Join(string.Empty, AlphaBetWorker.GetAlphabet("digital").GetTranslate(masks).Symbols.Select(x => x.AbSymbols[0]));
            Assert.Equal(list, result);
        }

        [Fact]
        public void Integration()
        {
            var cat = new BrailleCommand().BrOnly(new[] {"13", "153", "4523", "0"});
            Assert.Equal("⠅⠕⠞⠀", cat);

            var digAB = new BrailleCommand().Brs("digital");
            Assert.Equal(digAB, "1⠁\r\n2⠃\r\n3⠉\r\n4⠙\r\n5⠑\r\n6⠋\r\n7⠛\r\n8⠓\r\n9⠊\r\n0⠚");
        }


        [Theory]
        [MemberData(nameof(Brailles))]
        public void CheckBraille(string str, char c)
        {
            Assert.Equal(c, new BrailleSymbol(BrailleSymbol.GetMask(str)).Symbol);
        }

        public static IEnumerable<object[]> Brailles()
        {
            yield return new object[] { "0", '⠀' };
            yield return new object[] { "1", '⠁' };
            yield return new object[] { "1423", '⠏' };
            yield return new object[] { "65431", '⠽' };
        }

    }
}