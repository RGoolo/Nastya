using Model.Types.Interfaces;
using System;

namespace Web.Game.Model
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
		Guid IdMsg { get; }
		IUser User { get; }
		EventTypes EventType { get; }
		string Text { get; }
	}

	public class SimpleEvent : IEvent
	{
		public string Text { get; set; }
		public EventTypes EventType { get; set; }
		public Guid IdMsg { get; }
		public IUser User { get; }

		public SimpleEvent(EventTypes eventType, IUser user) : this (eventType, user, null, null)
		{

		}

		public SimpleEvent(EventTypes eventType, IUser user, string text, Guid? idMsg)
		{
			Text = text;
			EventType = eventType;
			IdMsg = idMsg.GetValueOrDefault();
			User = user;
		}
	}
}