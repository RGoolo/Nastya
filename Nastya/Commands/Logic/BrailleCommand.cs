using System;
using System.Linq;
using Model.Bots.BotTypes.Attribute;
using Model.Bots.BotTypes.Enums;
using Model.Logic.Braille;

namespace Nastya.Commands
{

    [CommandClass(nameof(BrailleCommand), "Брайль. Порядок ввода:\n1\t4\n2\t5\n3\t6\n", TypeUser.User)]
    public class BrailleCommand
    {
	    [Command(nameof(Br), "Брайль -> алфавит. /" + nameof(Br) + "_13_153_4523 -> ⠅К ⠕о ⠞т")]
        public string Br(string[] str) => string.Join("\n", AlphaBetWorker.GetTranslate(str).Select(x => x.ToString()));

        
        [Command(nameof(BrOnly), "Брайль -> алфавит. /" + nameof(BrOnly) + "_13_153_4523_0 -> ⠅⠕⠞⠀")]
        public string BrOnly(string[] str) => string.Join(string.Empty, AlphaBetWorker.GetBraille(str).Select(x => x.ToString()));

		
        [Command(nameof(Brs), "Вывести алфавиты:\n" + "/" + nameof(Brs) + "_ru\n" + "/" + nameof(Brs) + "_en\n" + "/" + nameof(Brs) + "_digital")]
		public string Brs(string name) => string.IsNullOrEmpty(name) ? Alphabets() : Alphabet(name);

		public string Alphabets() => string.Join(Environment.NewLine + Environment.NewLine, AlphaBetWorker.GetAlphabets().Select(a => Alphabet(a.Name)));
		public string Alphabet(string name) => $"{name}:"  + Environment.NewLine + string.Join("\t", AlphaBetWorker.GetAlphabet(name).SortAlphaBet);
    }
}