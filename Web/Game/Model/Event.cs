using System;

namespace Web.Game.Model
{
	/// <summary>
	/// В очередь положить 
	/// </summary>
	public enum EventTypes
	{
		Refresh,
		SendSpoiler,
		SendCode,
		SendCodes,
		GetLvlInfo,
		GetAllInfo,
		GetBonus,
		GetAllBonus,
		GetAllSectors,
		GetSectors,
		StartGame,
		StopGame,
		GetTimeForEnd,
		TakeBreak,
		GoToTheNextLevel,
	}

	public interface IEvent
	{
		EventTypes EventType { get; }
		string Text { get; }
		Guid IdMsg { get; }
	}

	public class SimpleEvent : IEvent
	{
		public string Text { get; set; }
		public EventTypes EventType { get; set; }
		public Guid IdMsg { get; }
		public SimpleEvent(EventTypes eventType, string text = "", Guid? idMsg = null)
		{
			Text = text;
			EventType = eventType;
			IdMsg = idMsg.GetValueOrDefault();
		}

	}
}