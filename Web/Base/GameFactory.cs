using Model.Logic.Settings;
using Web.DL;
using Web.DZR;

namespace Web.Base
{
    public static class GameFactory
    {
		public static IGame NewGame(ISettings settings, TypeGame type)
		{
			var typeGame = type;

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
