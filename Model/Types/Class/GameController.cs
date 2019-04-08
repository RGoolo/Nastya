using Model.Logic.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model.Types.Class
{
	public class GameController
	{
		private ISettings _settings { get; }

		public List<Answer> Answers { get; } = new List<Answer>();
		public string CurrentLvl { get; }

		public GameController(ISettings settings)
		{
			_settings = settings;
		}

		public CommandMessage SendSectors(string text, bool all, bool update)
		{
			var msgId = all ? _settings.Game.AllSectorsMsg : _settings.Game.SectorsMsg;
			var texter = new Texter(text, true);
		
			var msg = (msgId == Guid.Empty || update ) ? CommandMessage.GetTextMsg(texter) : CommandMessage.GetEditMsg(texter);
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
