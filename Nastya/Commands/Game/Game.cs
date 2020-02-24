using System.Collections.Generic;
using System.Linq;
using Model.BotTypes.Attribute;
using Model.BotTypes.Class;
using Model.BotTypes.Class.Ids;
using Model.BotTypes.Enums;
using Model.BotTypes.Interfaces;
using Model.BotTypes.Interfaces.Messages;
using Model.Logic.Settings;
using Web.Base;
using Web.Game.Model;

namespace Nastya.Commands
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
	[CommandClass("Game", "Дозор дедлайн", TypeUser.User)]
	public class Game1 : ISendSyncMsgs
	{
		private readonly ISendMessages _sendMessages;

		public Game1(ISendMessages sendMessages)
		{
			_sendMessages = sendMessages;
		}

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
		public string copyFromPM(ISettings chatSettings, IUser user)
		{
			var userSettings = SettingsHelper.GetSetting(user);
			chatSettings.Game.Password = userSettings.Game.Password;
			chatSettings.Game.Login = userSettings.Game.Login;
			chatSettings.SetUri(userSettings.Game.Site);

			return $"Скопировано. Логин = {userSettings.Game.Login}, сайт = {chatSettings.Game.Site}";
		}


		[Command(Const.Game.Level, "Номер задания, куда бить коды")]
		public string Level { get; set; }

		private IGame _game;

		public bool GameIsStart { get; private set; }

		[Command(Const.Game.Start, "Коннектится к сайту")]
		public void Start(IUser user, ISettings settings)
		{
			GetGame(settings)?.Start();
			//GameIsStart = true;
		}

		[CheckProperty(nameof(GameIsStart))]
		[Command(Const.Game.Stop, "Заканчивает игру")]
		public void Stop(IUser user, ISettings settings)
		{
			if (!GameIsStart)
				return;

			GetGame(settings)?.Stop();
			GameIsStart = false;
			_game = null;
		}

		[Command(Const.Game.Clear, "Удаляет игру из памяти.")]
		public void Clear()
		{
			//	GetGame()?.SetEvent(new SimpleEvent(EventTypes.c));
		}

		[CheckProperty(nameof(GameIsStart))]
		[Command(Const.Game.LvlText, "Прислать текст текущего уровня в чат", TypeUser.User)]
		public void LvlText(IUser user, ISettings settings)
		{
			GetGame(settings)?.SetEvent(new SimpleEvent(EventTypes.GetLvlInfo, user));
		}

		[CheckProperty(nameof(GameIsStart))]
		[Command(Const.Game.LvlAllText, "Прислать всю инфу по уровню", TypeUser.User)]
		public void LvlAllText(IUser user, ISettings settings)
		{
			GetGame(settings)?.SetEvent(new SimpleEvent(EventTypes.GetAllInfo, user));
		}

		[CheckProperty(nameof(GameIsStart))]
		[Command(Const.Game.LastCodes, "Оставшиеся сектора", TypeUser.User)]
		public void LastCodes(IUser user, ISettings settings)
		{
			GetGame(settings)?.SetEvent(new SimpleEvent(EventTypes.GetSectors, user));
		}

		[CheckProperty(nameof(GameIsStart))]
		[Command(Const.Game.Codes, "Секторы на уровне", TypeUser.User)]
		public void Codes(IUser user, ISettings settings)
		{
			GetGame(settings)?.SetEvent(new SimpleEvent(EventTypes.GetAllSectors, user));
		}

		[CheckProperty(nameof(GameIsStart))]
		[Command(Const.Game.Bonus, "Оставшиеся бонусы", TypeUser.User)]
		public void Bonus(IUser user, ISettings settings)
		{
			GetGame(settings)?.SetEvent(new SimpleEvent(EventTypes.GetBonus, user));
		}

		[CheckProperty(nameof(GameIsStart))]
		[Command(Const.Game.AllBonus, "Бонусы на уровне", TypeUser.User)]
		public void AllBonus(IUser user, ISettings settings)
		{
			GetGame(settings)?.SetEvent(new SimpleEvent(EventTypes.GetAllBonus, user));
		}

		[CheckProperty(nameof(GameIsStart))]
		[Command(Const.Game.UpdateBonuses, "Обновлять бонусы на уровне", TypeUser.User)]
		public bool UpdateBonuses { get; set; }


		[CheckProperty(nameof(GameIsStart))]
		[Command(Const.Game.UpdateAllBonuses, "Обновлять все бонусы на уровне", TypeUser.User)]
		public bool UpdateAllBonuses { get; set; }


		[CheckProperty(nameof(GameIsStart))]
		[Command(nameof(GoToTheNextLevel), "Перейти на следующий уровень")]
		public void GoToTheNextLevel(IUser user, ISettings settings)
		{
			GetGame(settings)?.SetEvent(new SimpleEvent(EventTypes.GoToTheNextLevel, user));
		}

		[CheckProperty(nameof(GameIsStart))]
		[Command(nameof(Time), "Сколько времени осталось", TypeUser.User)]
		public void Time(IUser user, ISettings settings)
		{
			GetGame(settings)?.SetEvent(new SimpleEvent(EventTypes.GetTimeForEnd, user));
		}

		[CheckProperty(nameof(GameIsStart))]
		[Command(nameof(TakeBreak), "Взять перерыв", TypeUser.Admin)]
		public void TakeBreak(IUser user, ISettings settings)
		{
			GetGame(settings)?.SetEvent(new SimpleEvent(EventTypes.TakeBreak, user));
		}

		[CheckProperty(nameof(GameIsStart))]
		[Command(nameof(Code), "Отправить код", TypeUser.User)]
		public void Code(IUser user, IBotMessage msg, ISettings settings, string code)
		{
			GetGame(settings)?.SendCode(code, msg.User, msg.MessageId);
		}

		// private ISettings SettingHelper => SettingsHelper.GetSetting(ChatId);

		private IGame GetGame(ISettings settings)
		{
			if (_game != null)
				return _game;

			_game = GameFactory.NewGame(settings, this);

			if (_game != null)
			{
				GameIsStart = true;
			}
			return _game;
		}

		public void SendSync(IEnumerable<IMessageToBot> messages)
		{
			var transaction = new TransactionCommandMessage(messages.ToList());
			_sendMessages.Send(transaction);
		}

		private void DeleteGame(IUser user, ISettings settings)
		{
			if (_game == null)
				return;

			//привет, параноя
			GetGame(settings)?.Stop();
			_game.Dispose();
			_game = null;
		}

		[CheckProperty(nameof(GameIsStart))]
		[CommandOnMsg(nameof(IsSendCoord), MessageType.Text, TypeUser.User)]
		public void Command(IBotMessage msg, ISettings settings)
		{
			if (msg.MessageCommands != null && msg.MessageCommands.Count() != 0)
				return;

			GetGame(settings)?.SendCode(msg.Text, msg.User, msg.MessageId);
		}

		public bool CheckSystemMsg => true;

		[CommandOnMsg(nameof(CheckSystemMsg), MessageType.All, TypeUser.Bot)]
		public void CheckSystem(IBotMessage msg, ISettings settings)
		{
			if (msg.ReplyToCommandMessage == null)
				return;

			switch(msg.ReplyToCommandMessage.Notification)
			{
				case Notification.SendAllSectors:
					settings.Game.AllSectorsMsg = msg.MessageId;
					break;
				case Notification.SendSectors:
					settings.Game.SectorsMsg = msg.MessageId;
					break;
				case Notification.GameStarted:
					GameIsStart = true;
					break;
				case Notification.GameStoped:
					Stop(msg.User, settings);
					break;
			};
		}
	}
}