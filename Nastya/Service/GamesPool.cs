using System;
using System.Collections.Generic;
using BotModel.Bots.BotTypes.Class;
using BotModel.Bots.BotTypes.Interfaces;
using BotModel.Bots.BotTypes.Interfaces.Ids;
using BotModel.Bots.BotTypes.Interfaces.Messages;
using BotModel.Exception;
using Model.Settings;
using Web.Entitiy;

namespace NightGameBot.Service
{
	public class GamesPool
	{
		private static readonly Dictionary<Guid, GameController> _games = new Dictionary<Guid, GameController>();
		private static IMessageId _messageId;
		private static IChatService _mainGameSettings;

		public IGameControl CreateGame(IChatService settings, ISenderSyncMsgs sendSyncMessage, IMessageId messageId)
		{
			_mainGameSettings = settings;
			var controller = new GameController(settings, sendSyncMessage);
			_messageId = messageId;
			_games.Add(controller.GameId, controller);
			return controller;
		}

		public IGame GetGame(Guid gameId, ISenderSyncMsgs sendSyncMessage, IBotMessage msg)
		{
			if (!_mainGameSettings.Game.AllowConnect)
				throw new GameException("Администратор запретил подключение к игре.");

			if (!_games.TryGetValue(gameId, out var game))
				throw new GameException("Игра не найдена");

			game.AddReceiver(sendSyncMessage);
			
			var text =  msg.Chat.Type == ChatType.Private 
				? $"К игре подключился {msg.User.Display}."
				: $"{msg.User.Display} добавил игру в чат: {msg.Chat.ChatName}.";

			var textMessage = MessageToBot.GetTextMsg(text);
			textMessage.OnIdMessage = _messageId;

			game.Send(textMessage);
			return game;
		}

		public void RemoveReceiver(Guid gameId, ISenderSyncMsgs sendSyncMessage)
		{
			if (!_games.TryGetValue(gameId, out var game))
				return;

			game.DeleteReceiver(sendSyncMessage);
		}
	}
}