using Model.Logic.Settings;
using System;
using System.Collections.Generic;
using System.Text;
using Web.Base;
using Web.DL;
using Web.DZR;
using Web.DZRLitePr;
using Web.Game.Model;

namespace Web.Base
{
    public static class GameFactory
    {
		static public IGame NewGame(ISettings settings, TypeGame type)
		{
			var typeGame = type;
			var typeGame2 = typeGame;

			if (typeGame == TypeGame.Unknown)
				return null;

			if ((typeGame & TypeGame.Dummy) != TypeGame.Unknown)
			{
				if ((typeGame & TypeGame.Dzzzr) != TypeGame.Unknown)
					return new Dzr(settings, typeGame);

				if ((typeGame & TypeGame.DeadLine) != TypeGame.Unknown)
					return new DLgame(settings, typeGame);
			}
			else
			{
				switch (typeGame)
				{
					case TypeGame.DzzzrLitePrequel:
						return new DZRLitePr.DZRLitePr(settings, typeGame);
						
					case TypeGame.Dzzzr:
						return new Dzr(settings, typeGame);
						
					case TypeGame.DeadLine:
						return new DLgame(settings, typeGame);
				}
			}
			return null;
		}

    }
}
