using Model.Bots.BotTypes.Interfaces.Ids;
using Model.Bots.BotTypes.Interfaces.Messages;

namespace Web.Entitiy
{

	public enum EventTypes
	{
		SendSpoiler,
		SendCode,
		GetLvlInfo,
		GetAllInfo,
		GetBonus,
		GetAllBonus,
		GetAllSectors,
		GetSectors,
		GetTimeForEnd,
		TakeBreak,
		GoToTheNextLevel,
	}

	public interface IEvent
	{
		IMessageId IdMsg { get; }
		IUser User { get; }
		EventTypes EventType { get; }
		string Text { get; }
	}

	public class SimpleEvent : IEvent
	{
		public string Text { get; set; }
		public EventTypes EventType { get; set; }
		public IMessageId IdMsg { get; }
		public IUser User { get; }

		public SimpleEvent(EventTypes eventType, IUser user) : this (eventType, user, null, null)
		{

		}

		public SimpleEvent(EventTypes eventType, IUser user, string text, IMessageId idMsg)
		{
			Text = text;
			EventType = eventType;
			IdMsg = idMsg;
			User = user;
		}
	}
}