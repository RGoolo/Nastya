using Model.Logic.Settings;
using Web.DL;
using Web.DZR;

namespace Web.Base
{
	public static class GameFactory
	{
		//ToDo: PoolThread


		public static IGame NewGame(ISettings settings, ISendSyncMsgs sendSyncMessage)
		{
			var typeGame = settings.TypeGame;

			if (typeGame == TypeGame.Unknown)
				return null;

			if ((typeGame & TypeGame.Dzzzr) != TypeGame.Unknown)
				return new ConcurrentGame(new DzrController(settings), sendSyncMessage);

			if ((typeGame & TypeGame.DeadLine) != TypeGame.Unknown)
				return new ConcurrentGame(new DlController(settings), sendSyncMessage);


			/*case TypeGame.DzzzrLitePrequel:
				return new DZRLitePr.DZRLitePr(settings, typeGame);*/

			return null;
		}
	}
}
