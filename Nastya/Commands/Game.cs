using System;
using System.Collections.Generic;
using System.Linq;
using Model.Logic.Settings;
using Model.Types.Attribute;
using Model.Types.Class;
using Model.Types.Interfaces;
using Web.Base;
using Web.Game.Model;

namespace Nastya.Commands
{
	public static class HelpText
	{
		public const string CustomHelp = "При первом запуске скопируйте сообщение со своими данными, команды можно менять местами.\n" +	   
					"/" + Const.Game.Uri + " http://classic.dzzzr.ru/demo/\n" +
					"/" + Const.Game.Login + " login\n" +
					"/" + Const.Game.Password + " \"password\"\n" +
					"/" + Const.Game.Prefix + " 1d\n" +
					"/" + Const.Game.Send + "_on\n";
	}

	[CustomHelp(HelpText.CustomHelp)]
	[CommandClass("Game", "Дозор дедлайн", Model.Types.Enums.TypeUser.User)]
	public class Game1 : BaseCommand, ISendSyncMsgs
	{
		[Command(Const.Game.Send, "Отправляет коды из чата. Если начать предложение с \"!\" будет спойлер")]
		public bool IsSendCoord { get; set; }

		[Command(Const.Game.Login, "Логин к игре")]
		public string Login { get; set; }

		[Command(Const.Game.Password, "Задать пароль к игре")]
		public string Password { get; set; }

		[Command(Const.Game.Uri, "Задать адрес игры")]
		public string Uri
		{ get => SettingsHelper.GetSetting(ChatId).Game.Uri; set => SettingsHelper.GetSetting(ChatId).SetUri(value);}

		[Command(Const.Game.Level, "Номер задания, куда бить коды")]
		public string Level { get; set; }

		[Command(Const.Game.Prefix, "Префикс кодов, для дозора")]
		public string Prefix { get; set; }

		[Command(Const.Game.CheckOtherTask, "Отслеживать все задание в ке, по умолчанию будет только верхнее")]
		public bool CheckOtherTask { get; set; }

		[Command(Const.Game.Sturm, "Если игра штурмавая, не забывайте указать уровень.")]
		public bool Sturm { get; set; }

		private IGame _game;

		public bool GameIsStart { get; private set; }

		[Command(Const.Game.Start, "Коннектится к сайту")]
		public void Start(IUser user)
		{
			GetGame()?.Start();
			//GameIsStart = true;
		}

		[CheckProperty(nameof(GameIsStart))]
		[Command(Const.Game.Stop, "Заканчивает игру")]
		public void Stop(IUser user)
		{
			if (!GameIsStart)
				return;

			GetGame()?.Stop();
			GameIsStart = false;
			_game = null;
		}

		[Command(Const.Game.Clear, "Удаляет игру из памяти.")]
		public void Clear()
		{
			//	GetGame()?.SetEvent(new SimpleEvent(EventTypes.c));
		}

		[CheckProperty(nameof(GameIsStart))]
		[Command(Const.Game.LvlText, "Прислать текст текущего уровня в чат", Model.Types.Enums.TypeUser.User)]
		public void LvlText(IUser user)
		{
			GetGame()?.SetEvent(new SimpleEvent(EventTypes.GetLvlInfo, user));
		}

		[CheckProperty(nameof(GameIsStart))]
		[Command(Const.Game.LvlAllText, "Прислать всю инфу по уровню", Model.Types.Enums.TypeUser.User)]
		public void LvlAllText(IUser user)
		{
			GetGame()?.SetEvent(new SimpleEvent(EventTypes.GetAllInfo, user));
		}

		[CheckProperty(nameof(GameIsStart))]
		[Command(Const.Game.LastCodes, "Оставшиеся сектора", Model.Types.Enums.TypeUser.User)]
		public void LastCodes(IUser user)
		{
			GetGame()?.SetEvent(new SimpleEvent(EventTypes.GetSectors, user));
		}

		[CheckProperty(nameof(GameIsStart))]
		[Command(Const.Game.Codes, "Секторы на уровне", Model.Types.Enums.TypeUser.User)]
		public void Codes(IUser user)
		{
			GetGame()?.SetEvent(new SimpleEvent(EventTypes.GetAllSectors, user));
		}

		[CheckProperty(nameof(GameIsStart))]
		[Command(Const.Game.Bonus, "Оставшиеся бонусы", Model.Types.Enums.TypeUser.User)]
		public void Bonus(IUser user)
		{
			GetGame()?.SetEvent(new SimpleEvent(EventTypes.GetBonus, user));
		}

		[CheckProperty(nameof(GameIsStart))]
		[Command(Const.Game.AllBonus, "Бонусы на уровне", Model.Types.Enums.TypeUser.User)]
		public void AllBonus(IUser user)
		{
			GetGame()?.SetEvent(new SimpleEvent(EventTypes.GetAllBonus, user));
		}

		[CheckProperty(nameof(GameIsStart))]
		[Command(nameof(GoToTheNextLevel), "Перейти на следующий уровень")]
		public void GoToTheNextLevel(IUser user)
		{
			GetGame()?.SetEvent(new SimpleEvent(EventTypes.GoToTheNextLevel, user));
		}

		[CheckProperty(nameof(GameIsStart))]
		[Command(nameof(Time), "Сколько времени осталось", Model.Types.Enums.TypeUser.User)]
		public void Time(IUser user)
		{
			GetGame()?.SetEvent(new SimpleEvent(EventTypes.GetTimeForEnd, user));
		}

		[CheckProperty(nameof(GameIsStart))]
		[Command(nameof(TakeBreak), "Взять перерыв", Model.Types.Enums.TypeUser.Admin)]
		public void TakeBreak(IUser user)
		{
			GetGame()?.SetEvent(new SimpleEvent(EventTypes.TakeBreak, user));
		}

		[CheckProperty(nameof(GameIsStart))]
		[Command(nameof(Code), "Отправить код", Model.Types.Enums.TypeUser.User)]
		public void Code(IUser user, IMessage msg, string code)
		{
			GetGame()?.SendCode(code, msg.User, msg.MessageId);
		}

		private ISettings SettingHelper => SettingsHelper.GetSetting(ChatId);

		private IGame GetGame()
		{
		
			if (_game != null)
				return _game;

			_game = GameFactory.NewGame(SettingHelper, this);

			if (_game != null)
			{
				GameIsStart = true;
			}
			return _game;
		}

		public void SendSync(IEnumerable<CommandMessage> messages)
		{
			var transaction = new TransactionCommandMessage(messages.ToList());
			SendMsg.SendMsg(transaction);
		}


		private void DeleteGame(IUser user)
		{
			if (_game == null)
				return;

			//привет, параноя
			GetGame()?.Stop();
			_game.Dispose();
			_game = null;
		}

		[CheckProperty(nameof(GameIsStart))]
		[CommandOnMsg(nameof(IsSendCoord), Model.Types.Enums.MessageType.Text, Model.Types.Enums.TypeUser.User)]
		public void Command(IMessage msg)
		{
			if (msg.MessageCommands != null && msg.MessageCommands.Count() != 0)
				return;

			GetGame()?.SendCode(msg.Text, msg.User, msg.MessageId);
		}

		public bool CheckSystemMsg => true;

	

		[CommandOnMsg(nameof(CheckSystemMsg), Model.Types.Enums.MessageType.SystemMessage, Model.Types.Enums.TypeUser.User)]
		public void CheckSystem(Model.Types.Interfaces.IMessage msg)
		{
			if (msg.ReplyToCommandMessage == null)
				return;

			switch(msg.ReplyToCommandMessage.Notification)
			{
				case Model.Types.Enums.Notification.SendAllSectors:
					SettingHelper.Game.AllSectorsMsg = msg.MessageId;
					break;
				case Model.Types.Enums.Notification.SendSectors:
					SettingHelper.Game.SectorsMsg = msg.MessageId;
					break;
				case Model.Types.Enums.Notification.GameStarted:
					GameIsStart = true;
					break;
				case Model.Types.Enums.Notification.GameStoped:
					Stop(msg.User);
					break;
			};
		}

	
	}
}