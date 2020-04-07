using System.Collections.Generic;
using Model.Logic.Settings;

namespace Model.Bots.BotTypes.Class
{
	public class GameController
	{
		private readonly ISettings _settings;

		public List<Answer> Answers { get; } = new List<Answer>();
		public string CurrentLvl { get; }

		public GameController(ISettings settings)
		{
			_settings = settings;
		}

		public IMessageToBot SendSectors(string text, bool all, bool update)
		{
			var msgId = all ? _settings.Game.AllSectorsMsg : _settings.Game.SectorsMsg;
			var texter = new Texter(text, true);
		
			var msg = (msgId == null|| update ) ? MessageToBot.GetTextMsg(texter) : MessageToBot.GetEditMsg(texter);
			msg.Notification = all ? Enums.Notification.SendAllSectors : Enums.Notification.SendSectors;
			if (!update)
				msg.OnIdMessage = msgId;
			return msg;
		}

		public void SetGameAnswer(Answer answer)
		{
			Answers.Add(answer);
			//_settings.Answers.Add(answer);
		}

		public void SetNewLvl(string lvl)
		{
			if (string.IsNullOrEmpty(lvl) || lvl == CurrentLvl)
				return;

			//var a = _settings.Answers.Where(x => x.Lvl)
			Answers.Clear();
		}
	}
}
