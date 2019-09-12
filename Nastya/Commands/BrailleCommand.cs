using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model.Logic.Braille;
using Model.Types.Attribute;

namespace Nastya.Commands
{

    [CommandClass(nameof(SyntacticalAnalyzer), "Брайль. Порядок:\n1\t4\n2\t5\n3\t6\n", Model.Types.Enums.TypeUser.User)]
    public class BrailleCommand
    {

        [Command(nameof(Br), "Брайль -> алфавит. /" + nameof(Br) + "_13_153_4523 -> Кот" )]
        public string Br(string[] str) => string.Join("\n", AlphaBetWorker.GetTranslate(str).Select(x => x.ToString()));

        [Command(nameof(Brailles), "Брайль -> алфавит. /" + nameof(Brailles) + "_13_153_4523_0 -> ⠅⠕⠞⠀")]
        public string Brailles(string[] str) => string.Join(string.Empty, AlphaBetWorker.GetBraille(str).Select(x => x.ToString()));

        [Command(nameof(Alphabet), "Показать алфавит:\n" + nameof(Brailles) + "_ru\n" + nameof(Brailles) + "_ru\n" + nameof(Brailles) + "_digital")]
        public string Alphabet(string str) => string.Join(Environment.NewLine, AlphaBetWorker.GetAlphabet(str).SortAlphaBet);
    }
}