using System.Collections.Generic;
using System.Linq;
using Model.Logic.Braille;
using NightGameBot.Commands.Logic;
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
            Assert.Equal(digAB, "digital:\r\n1⠁\t2⠃\t3⠉\t4⠙\t5⠑\t6⠋\t7⠛\t8⠓\t9⠊\t0⠚");
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