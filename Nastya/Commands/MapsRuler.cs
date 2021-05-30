using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BotModel.Bots.BotTypes.Attribute;
using BotModel.Bots.BotTypes.Class;
using BotModel.Bots.BotTypes.Enums;
using BotModel.Bots.BotTypes.Interfaces.Messages;
using GoogleApi;
using GoogleApi.Entities.Common.Enums;
using GoogleApi.Entities.Maps.Directions.Request;
using Model.Settings;

namespace NightGameBot.Commands
{

	[CommandClass(nameof(MapsRuler), "Интеграция с сайтом. Можно по картам смотреть кто-где находится и управлять людьми. Необходимо расшарить геолокациб в общий чат.", TypeUser.User)]
	public class MapsRuler
	{
		private readonly IChatService _service;

		private class IUserComparer : IEqualityComparer<IUser>
		{
			public bool Equals(IUser x, IUser y)
			{
				return x.Id.Equals(y.Id);
			}

			public int GetHashCode(IUser obj)
			{
				return obj.Id.GetHashCode();
			}
		}

		private enum CurrentState
		{
			Undefined,
			OnWay,
			Arrived,
			Wait,
		}

		public MapsRuler(IChatService service)
		{
			_service = service;
		}

		private class UserLocations
		{
			public UserLocations(IUser user)
			{
				User = user;
			}

			public IUser User { get; set; }
			public Location LocationTo { get; set; }
			public Location LocationNow { get; set; }
			public CurrentState CurrentState { get; set; }

			public bool IsTwoLocation() => LocationNow != null && LocationTo != null;
		}

		private Dictionary<Guid, UserLocations> _users = new ();

		private string CallbackDataOtsleshivat = "qwaseqty22";
		private string CallbackDataNeOtsleshivat = "Noqe3erty22";
		private string MainText = "Что бы добавить телефон для отслеживания расшарте локацию в телеграмме и нажмите на кнопку снизу.";

		private string CallbackDataAll = "asdwq3lwq3";
		private string CallbackDataUser = "qwesad_";

		private string CallbackDataState = "asdw2_";

		private string GetTextMessage()
		{
			var sb = new StringBuilder(MainText);
			if (_users.Count > 0)
			{
				sb.AppendLine("Подключены:");
				foreach (var user in _users.Select(u => u.Value.User))
				{
					sb.Append(user.Display);
				}
			}

			return sb.ToString();
		}

		private IMessageToBot CreateMessageEdit(IBotMessage botMessage)
		{
			var msg = MessageToBot.GetEditMsg(new Texter(GetTextMessage()));
			msg.EditMsg = botMessage.ReplyToMessage.MessageId;
			msg.QueryBotMessage = new QueryBotMessage()
				.Add(("Отслеживать", CallbackDataOtsleshivat))
				.Add(("Не отслеживать", CallbackDataNeOtsleshivat));
			return msg;
		}

		private IMessageToBot CreateMessage()
		{
			var msg = MessageToBot.GetTextMsg(GetTextMessage());
			msg.QueryBotMessage = new QueryBotMessage()
				.Add(("Отслеживать", CallbackDataOtsleshivat))
				.Add(("Не отслеживать", CallbackDataNeOtsleshivat)); ;
			return msg;
		}

		//private Guid LastMessageId;
		// [Command(nameof(StartCarOptions), "Позволять присоединятся к игре в других чатах")]
		public bool StartCarOptions { get; set; } = true;

		[Command(nameof(StartCar), "Добавить отслеживание телефонов на карте.", TypeUser.User)]
		public IMessageToBot StartCar()
		{
			StartCarOptions = true;
			return CreateMessage();
		}

		[Command(nameof(EndCar), "Завершить отслеживание телефонов на карте.", TypeUser.User)]
		public string EndCar()
		{
			StartCarOptions = false;
			return "Завершено отслеживание";
		}

		[CommandOnMsg(nameof(StartCarOptions), MessageType.CallBack, TypeUser.User)]
		public async Task<IMessageToBot> StartCarOptions2(IBotMessage msg)
		{
			if (msg.Text == CallbackDataOtsleshivat)
			{
				_users.TryAdd(msg.User.Id, new UserLocations(msg.User));
				return CreateMessageEdit(msg);
			}

			if (msg.Text == CallbackDataNeOtsleshivat)
			{
				_users.Remove(msg.User.Id);
				return CreateMessageEdit(msg);
			}

			if (msg.Text.StartsWith(CallbackDataState))
			{
				var state =  Enum.Parse<CurrentState>(msg.Text.Split("_")[1]);
				if (_users.TryGetValue(msg.User.Id, out var userInfo))
				{
					userInfo.CurrentState = state;

					if (userInfo.IsTwoLocation())
						return MessageToBot.GetTextMsg(await TextForUser(userInfo));
				}
			}

			return null;
		}

		[CommandOnMsg(nameof(StartCarOptions), MessageType.CallBack, TypeUser.Admin)]
		public IMessageToBot StartCarOptions3(IBotMessage msg, IChatService settings)
		{
			if (msg.Text == CallbackDataAll)
			{
				foreach (var user in _users.Values)
				{
					user.LocationTo = msg.Location;
				}
				
				var msg2 = MessageToBot.GetTextMsg("пиздуйте все сюда:");
				msg2.QueryBotMessage = CreateQuery2();
				return msg2;
			}

			if (msg.Text.StartsWith(CallbackDataUser))
			{
				var userId = new Guid(msg.Text.Split("_")[1]);
				if (_users.TryGetValue(userId, out var userInfo))
				{
					userInfo.LocationTo = msg.Location;
					var msg2  = MessageToBot.GetTextMsg($"пиздуй {userInfo.User.Display} сюда:");
					msg2.QueryBotMessage = CreateQuery2();
					return msg2;
				}

				// return CreateMessageEdit(msg);
			}

			return null;
		}

		private Task<string> TextForUser(UserLocations info)
		{
			return TextForUser(info, info.LocationTo);
		}

		private async Task<string> TextForUser(UserLocations info, Location to)
		{
			var request = new DirectionsRequest
			{
				Key = _service.Coordinates.GoogleCred,
				Origin = new GoogleApi.Entities.Common.Location(info.LocationNow.Latitude, info.LocationNow.Longitude),
				Destination = new GoogleApi.Entities.Common.Location(to.Latitude, to.Longitude),
				Language = Language.Russian,
			};

			var result = await GoogleMaps.Directions.QueryAsync(request);
			var leg = result.Routes.First().Legs.First();

			return $"{info.User.Display}: {leg.StartAddress} - {leg.EndAddress}, расчетное время поездки: {leg.Duration.Text} ";
			//var polyline = result.Routes.First().Legs.First().Steps.First().PolyLine;
		}

		[CommandOnMsg(nameof(StartCarOptions), MessageType.Location | MessageType.Coordinates, TypeUser.Bot)]
		public async Task<IMessageToBot> StartCarOptions4(IBotMessage msg, IChatService settings)
		{
			if (_users.Count == 0)
				return null;

			var text = new StringBuilder("Отправить экипажи:").AppendLine();
			foreach (var usr in _users.Values.Where(u => u.LocationNow != null))
				text.AppendLine(await TextForUser(usr, msg.Location));
			
			var msg2 = MessageToBot.GetTextMsg(text.ToString());
			msg2.OnIdMessage = msg.MessageId;
			msg2.QueryBotMessage = CreateQuery();
			return msg2;
		}

		[CommandOnMsg(nameof(StartCarOptions), MessageType.Coordinates | MessageType.Edit, TypeUser.User)]
		public void StartCarOptions5(IUser usr, IBotMessage msg)
		{
			if (_users.TryGetValue(usr.Id, out var info))
				info.LocationNow = msg.Location;
		}

		private QueryBotMessage CreateQuery2()
		{
			if (_users.Count == 0)
				return null;

			return new QueryBotMessage().Add("Еду", CallbackDataState + CurrentState.OnWay)
				.Add("Прибыл", CallbackDataState + CurrentState.Arrived)
				.Add("Ожидаю", CallbackDataState + CurrentState.Wait);
		}

		private QueryBotMessage CreateQuery()
		{
			if (_users.Count == 0)
				return null;

			var q = new QueryBotMessage();
			q.Add(("Отправить все экипажи", CallbackDataAll));
			foreach (var usr in _users.Select(u => u.Value.User))
			{
				q.Add(($"Отправить {usr.Display}", CallbackDataUser + usr.Id));
			}

			return q;
		}
	}
}