using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotModel.Bots.BotTypes.Class;
using BotModel.Bots.BotTypes.Interfaces.Ids;
using BotModel.Bots.BotTypes.Interfaces.Messages;
using Model.Settings;
using Web.Entitiy;

namespace NightGameBot.Service
{
	public class GameController : IGameControl, ISenderSyncMsgs
	{
		private readonly IGameControl _game;
		private readonly IChatService _mainGameSettings;
		private readonly IList<ISenderSyncMsgs> _senders = new List<ISenderSyncMsgs>();

		public GameController(IChatService mainGameSettings, ISenderSyncMsgs sendSyncMessage)
		{
			ChatId = sendSyncMessage.ChatId;
			var game = GameFactory.NewGame(mainGameSettings, this);

			_game = game;
			_mainGameSettings = mainGameSettings;
			_senders.Add(sendSyncMessage);
		}

		public void Dispose()
		{
			_game.Dispose();
		}

		public Guid GameId => _game.GameId;

		public void SetEvent(IEvent iEvent)
		{
			_game.SetEvent(iEvent);
		}

		public void SendCode(string code, IUser user, IMessageId replaceMsg)
		{
			if (_mainGameSettings.Game.Send)
				_game.SendCode(code, user, replaceMsg);
		}

		public void AddReceiver(ISenderSyncMsgs send)
		{
			_senders.Add(send);
		}

		public void DeleteReceiver(ISenderSyncMsgs send)
		{
			_senders.Remove(send);
		}

		public Task Start()
		{
			return _game.Start();
		}

		public void Stop()
		{
			_game.Stop();
		}

		public IChatId ChatId { get; }

		/*public void SendSync(IList<IMessageToBot> messages)
		{
			if (messages == null || messages.Count == 0) return;

			foreach (var message in messages)
			{
				if (message.OnIdMessage == null && message.EditMsg == null)
				{
					foreach (var sender in _senders)
					{
						sender.SendSync(message);
					}
				}
			}
		}*/

		public void Send(IMessageToBot message)
		{
			if (message.OnIdMessage == null && message.EditMsg == null)
			{
				foreach (var sender in _senders)
					sender.Send(message);
				return;
			}

			if (message.OnIdMessage != null)
			{
				var msg = _senders.FirstOrDefault(s => Equals(s.ChatId, message.OnIdMessage.ChatId));
				msg?.Send(message);
				return;
			}

			if (message.EditMsg != null)
			{
				_senders.FirstOrDefault(s => Equals(s.ChatId, message.EditMsg.ChatId))?.Send(message);
				return;
			}
		}
	}
}