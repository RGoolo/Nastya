using System;
using Google.Cloud.Vision.V1;
using Model.Logic;
using Model.Logic.Braille;

namespace Model.Logic.Braille
{
    internal class AlphaBetRu : AlphaBet
    {
        public override string Name => "ru";

        protected override string LazyFileBraille => @"⠁	А
⠃	Б
⠺	В
⠛	Г
⠙	Д
⠑	Е
⠡	Ё
⠚	Ж
⠵	З
⠊	И
⠯	Й
⠅	К
⠇	Л
⠍	М
⠝	Н
⠕	О
⠏	П
⠗	Р
⠎	С
⠞	Т
⠥	У
⠋	Ф
⠓	Х
⠉	Ц
⠟	Ч
⠱	Ш
⠭	Щ
⠷	Ъ
⠮	Ы
⠾	Ь
⠪	Э
⠳	Ю
⠫	Я";
    }

    internal class AlphaBetEn : AlphaBet
    {


        public override string Name => "en";

        protected override string LazyFileBraille => @"⠁	A
⠃	B
⠉	C
⠙	D
⠑	E
⠋	F
⠛	G
⠓	H
⠊	I
⠚	J
⠅	K
⠇	L
⠍	M
⠝	N
⠕	O
⠏	P
⠟	Q
⠗	R
⠎	S
⠞	T
⠥	U
⠧	V
⠺	W
⠭	X
⠽	Y
⠵	Z
⠼	№
⠲	.
⠂	,
⠢	?
⠆	;
⠤	-";
	}
}

internal class AlphaBetDigital : AlphaBet
{


    public override string Name => "digital";

    protected override string LazyFileBraille => @"⠁	1
⠃	2
⠉	3
⠙	4
⠑	5
⠋	6
⠛	7
⠓	8
⠊	9
⠚	0";
}

