using System;
using Model.Bots.BotTypes.Enums;
using Model.Logic.Settings;
using Web.Base;
using Web.DL;
using Web.DZR;

namespace Web.Entitiy
{
	public static class GameFactory
	{
		//ToDo: PoolThread


		public static IGameControl NewGame(ISettings settings, ISenderSyncMsgs sendSyncMessage)
		{
			var typeGame = settings.TypeGame;

			if (typeGame == TypeGame.Unknown)
				return null;

			if ((typeGame & TypeGame.Dzzzr) != TypeGame.Unknown)
				return new ConcurrentGame(new DzrController(settings), sendSyncMessage, Guid.NewGuid());

			if ((typeGame & TypeGame.DeadLine) != TypeGame.Unknown)
				return new ConcurrentGame(new DlController(settings), sendSyncMessage, Guid.NewGuid());


			/*case TypeGame.DzzzrLitePrequel:
				return new DZRLitePr.DZRLitePr(settings, typeGame);*/

			return null;
		}
	}
}
