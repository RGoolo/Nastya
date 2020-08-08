using System;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf;
using Model.Bots.BotTypes.Attribute;
using Model.Bots.BotTypes.Class;
using Model.Bots.BotTypes.Enums;
using Model.Bots.BotTypes.Interfaces.Ids;
using Model.Bots.BotTypes.Interfaces.Messages;
using Model.Logic.Settings;
using Nastya.Service;
using Web.Entitiy;

namespace Nastya.Commands.Game
{
	public static class HelpText
	{
		public const string CustomHelp = "При первом запуске скопируйте сообщение со своими данными, команды можно менять местами.\n" +	   
					"/" + Const.Game.Site + " http://classic.dzzzr.ru/demo/\n" +
					"/" + Const.Game.Login + " login\n" +
					"/" + Const.Game.Password + " \"password\"\n" +
					"/" + Const.Game.Send + "_on\n";
	}

	[CustomHelp(HelpText.CustomHelp)]
	[CommandClass("Game", "Дозор/Дедлайн", TypeUser.User)]
	public class Game1 : ISenderSyncMsgs
	{
		private readonly ISendMessages _sendMessages;
		private readonly ISettings _settings;

		public Game1(ISendMessages sendMessages, IChatId chatId, ISettings settings)
		{
			_gamePool = new GamesPool();
			_sendMessages = sendMessages;
			_settings = settings;
			ChatId = chatId;
		}

		private readonly GamesPool _gamePool;
		private IControl _control;
		private IGame _game;

		[Command(nameof(Connect), "Присоединиться к игре")]
		public string Connect(Guid gameId, IBotMessage msg)
		{
			_game = _gamePool.GetGame(gameId, this, msg);
			if (_game == null) return "Не удалось подключится";
			GameIsStart = true;
			return "подключилась";
		}

		[Command(nameof(Const.Game.AllowConnect), "Позволять присоединятся к игре в других чатах")]
		public bool AllowConnect { get; set; }

        [Command(nameof(Const.Game.AllowCodeAudio), "Распозновать коды с голосовых.")]
        public bool AllowCodeSound { get; set; }

		[Command(Const.Game.Send, "Отправляет коды из чата.")]
		public bool IsSendCoord { get; set; }

		[Command(Const.Game.Login, "Логин к игре")]
		public string Login { get; set; }

		[Password]
		[Command(Const.Game.Password, "Задать пароль к игре")]
		public string Password { get; set; }

		[Command(Const.Game.Site, "Задать адрес игры")]
		public string Site(IChatId chatId, string url)
		{
			var  type = SettingsHelper.GetSetting(chatId).SetUri(url);
			return $"type={type}\nurl={url}";
		}

		[Command(nameof(Const.Game.CopyFromPM), "Скопировать данные по игре с лички (логин, пароль, сайт).", TypeUser.Admin)]
		public string CopyFromPM(ISettings chatSettings, IUser user)
		{
			var userSettings = SettingsHelper.GetSetting(user);
			chatSettings.Game.Password = userSettings.Game.Password;
			chatSettings.Game.Login = userSettings.Game.Login;
			chatSettings.SetUri(userSettings.Game.Site);

			return $"Скопировано. Логин = {userSettings.Game.Login}, сайт = {chatSettings.Game.Site}";
		}

		[Command(Const.Game.Level, "Номер задания, куда бить коды")]
		public string Level { get; set; }

		public bool GameIsStart { get; private set; }

		[Command(Const.Game.Start, "Коннектится к сайту")]
		public void Start(IUser user, IBotMessage messageBot)
		{
			var gc = _gamePool.CreateGame(_settings, this, messageBot.MessageId);

			_game = gc;
			_control = gc;
			_control.Start();

			GameIsStart = true;
		}

		[CheckProperty(nameof(GameIsStart))]
		[Command(Const.Game.Stop, "Заканчивает игру")]
		public void Stop(IUser user)
		{
			GameIsStart = false;

			if (_game != null)
			{
				_gamePool.RemoveReceiver(_game.GameId, this);
				_game = null;
			}

			_control?.Stop();
			_control = null;
		}

		[Command(Const.Game.Clear, "Удаляет игру из памяти.")]
		public void Clear()
		{
			//	GetGame()?.SetEvent(new SimpleEvent(EventTypes.c));
		}

		[CheckProperty(nameof(GameIsStart))]
		[Command(Const.Game.LvlText, "Прислать текст текущего уровня в чат", TypeUser.User)]
		public void LvlText(IUser user)
		{
			GetGame()?.SetEvent(new SimpleEvent(EventTypes.GetLvlInfo, user));
		}

		[CheckProperty(nameof(GameIsStart))]
		[Command(Const.Game.LvlAllText, "Прислать всю инфу по уровню", TypeUser.User)]
		public void LvlAllText(IUser user)
		{
			GetGame()?.SetEvent(new SimpleEvent(EventTypes.GetAllInfo, user));
		}

		[CheckProperty(nameof(GameIsStart))]
		[Command(Const.Game.LastCodes, "Оставшиеся сектора", TypeUser.User)]
		public void LastCodes(IUser user)
		{
			GetGame()?.SetEvent(new SimpleEvent(EventTypes.GetSectors, user));
		}

		[CheckProperty(nameof(GameIsStart))]
		[Command(Const.Game.Codes, "Секторы на уровне", TypeUser.User)]
		public void Codes(IUser user)
		{
			GetGame()?.SetEvent(new SimpleEvent(EventTypes.GetAllSectors, user));
		}

		[CheckProperty(nameof(GameIsStart))]
		[Command(Const.Game.Bonus, "Оставшиеся бонусы", TypeUser.User)]
		public void Bonus(IUser user)
		{
			GetGame()?.SetEvent(new SimpleEvent(EventTypes.GetBonus, user));
		}

		[CheckProperty(nameof(GameIsStart))]
		[Command(Const.Game.AllBonus, "Бонусы на уровне", TypeUser.User)]
		public void AllBonus(IUser user)
		{
			GetGame()?.SetEvent(new SimpleEvent(EventTypes.GetAllBonus, user));
		}

		[CheckProperty(nameof(GameIsStart))]
		[Command(Const.Game.UpdateBonuses, "Обновлять бонусы на уровне", TypeUser.User)]
		public bool UpdateBonuses { get; set; }


		[CheckProperty(nameof(GameIsStart))]
		[Command(Const.Game.UpdateAllBonuses, "Обновлять все бонусы на уровне", TypeUser.User)]
		public bool UpdateAllBonuses { get; set; }


		[CheckProperty(nameof(GameIsStart))]
		[Command(nameof(GoToTheNextLevel), "Перейти на следующий уровень")]
		public void GoToTheNextLevel(IUser user)
		{
			GetGame()?.SetEvent(new SimpleEvent(EventTypes.GoToTheNextLevel, user));
		}

		[CheckProperty(nameof(GameIsStart))]
		[Command(nameof(Time), "Сколько времени осталось", TypeUser.User)]
		public void Time(IUser user)
		{
			GetGame()?.SetEvent(new SimpleEvent(EventTypes.GetTimeForEnd, user));
		}

		[CheckProperty(nameof(GameIsStart))]
		[Command(nameof(TakeBreak), "Взять перерыв", TypeUser.Admin)]
		public void TakeBreak(IUser user)
		{
			GetGame()?.SetEvent(new SimpleEvent(EventTypes.TakeBreak, user));
		}

		[CheckProperty(nameof(GameIsStart))]
		[Command(nameof(Code), "Отправить код", TypeUser.User)]
		public void Code(IBotMessage msg, string code)
		{
			GetGame()?.SendCode(code, msg.User, msg.MessageId);
		}

		// private ISettings SettingHelper => SettingsHelper.GetSetting(ChatId);
		private IGame GetGame(Guid? gameId = null, IMessage msg = null, IMessageId messageId = null)
		{
			return _game;
		}

		public IChatId ChatId { get; }

		public void Send(IMessageToBot messages)
		{
			var transaction = new TransactionCommandMessage(messages);
			_sendMessages.Send(transaction);
		}

		public void SendSync(IList<IMessageToBot> messages)
		{
			var transaction = new TransactionCommandMessage(messages.ToList());
			_sendMessages.Send(transaction);
		}

	
		[CheckProperty(nameof(GameIsStart))]
		[CommandOnMsg(nameof(IsSendCoord), MessageType.Text, TypeUser.User)]
		public void Command(IBotMessage msg, ISettings settings)
		{
			if (msg.MessageCommands != null && msg.MessageCommands.Count() != 0)
				return;

			GetGame()?.SendCode(msg.Text, msg.User, msg.MessageId);
		}

        [CheckProperty(nameof(GameIsStart))]
        [CheckProperty(nameof(AllowCodeSound))]
		[CommandOnMsg(nameof(IsSendCoord), MessageType.Text, TypeUser.Bot)]
        public void Command2(IBotMessage msg, ISettings settings)
        {
            if (msg.MessageCommands != null && msg.MessageCommands.Count() != 0)
                return;

			if (msg.ReplyToMessage == null || msg.ReplyToMessage.TypeMessage != MessageType.Voice)
                return;

            if (string.IsNullOrEmpty(msg.Text))
                return;
            
            string text = null;
            
            if (msg.Text.StartsWith("код "))
                text = msg.Text.Substring(3).Replace(" ", "");
            
			if (!string.IsNullOrEmpty(text))
                GetGame()?.SendCode(text, msg.User, msg.MessageId);
        }

		public bool CheckSystemMsg => true;

		[CommandOnMsg(nameof(CheckSystemMsg), MessageType.All, TypeUser.Bot)]
		public void CheckSystem(IBotMessage msg)
		{
			if (msg.ReplyToCommandMessage == null)
				return;

			switch(msg.ReplyToCommandMessage.Notification)
			{
				case Notification.SendAllSectors:
					_settings.Game.AllSectorsMsg = msg.MessageId;
					break;
				case Notification.SendSectors:
					_settings.Game.SectorsMsg = msg.MessageId;
					break;
				case Notification.GameStarted:
					GameIsStart = true;
					break;
				case Notification.GameStoped:
					Stop(msg.User);
					break;
			};
		}
	}
}