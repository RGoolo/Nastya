using Model.Types.Interfaces;
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
		IUser User { get; }
	}

	public class SimpleEvent : IEvent
	{
		public string Text { get; set; }
		public EventTypes EventType { get; set; }
		public Guid IdMsg { get; }
		public IUser User { get; }

		public SimpleEvent(EventTypes eventType, IUser user, string text = "", Guid? idMsg = null)
		{
			Text = text;
			EventType = eventType;
			IdMsg = idMsg.GetValueOrDefault();
			User = user;
		}

	}
}