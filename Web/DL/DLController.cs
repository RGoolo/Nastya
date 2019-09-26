using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Model.Logic.Coordinates;
using Model.Logic.Model;
using Model.Logic.Settings;
using Model.Types.Class;
using Model.Types.Enums;
using Model.Types.Interfaces;
using Web.Base;
using Web.DL.PageTypes;
using Web.Game.Model;

namespace Web.DL
{

	public interface ISendMsgDl
	{
		void SendMsg(IEnumerable<CommandMessage> msgs);
	}

	public class DlController : IController, ISendMsgDl
	{
	
		public event SendMsgsSyncDel SendMsgs;

		public ISettings Setting { get; }

		private DlWebValidator _webValidator { get; }
		public TypeGame TypeGame => Setting.TypeGame;

		public ISettings Settings => Setting;

		private PageController _pageController;

		public DlController(ISettings setting)// : base(setting)
		{
			Setting = setting;
			_webValidator = new DlWebValidator(setting);
			_pageController = new PageController(this, setting);
		}

		public void SendEvent(IEvent iEvent)
		{
			switch (iEvent.EventType)
			{
				case EventTypes.GetLvlInfo:
				case EventTypes.GetAllInfo:
					_pageController.SendNewLevelInfo(_pageController._lastPage);
					break;
				case EventTypes.GetBonus:
					_pageController.SendBonus(_pageController._lastPage, false);
					break;
				case EventTypes.GetAllBonus:
					_pageController.SendBonus(_pageController._lastPage, true);
					break;

				case EventTypes.GetAllSectors:
					_pageController.SendSectors(_pageController._lastPage);
					break;
				case EventTypes.GetSectors:
					_pageController.SendSectors(_pageController._lastPage, true);
					break;

				case EventTypes.GetTimeForEnd:
					_pageController.SendMsg($"Времени до автоперехода: {_pageController._lastPage.TimeToEnd?.ToString(PageController.TimeFormat)}");
					break;

				case EventTypes.SendCode:
					_pageController.AfterSendCode(_webValidator.SendCode(iEvent.Text, _pageController._lastPage), iEvent);
					break;
			}
		}

		public void LogIn() => _pageController.SetNewPage(_webValidator.LogIn());

		public bool IsLogOut() => _webValidator.IsLogOut(_pageController._lastPage);

		public List<IEvent> GetCode(string str, IUser user, Guid replaceMsg)
		{
			if (string.IsNullOrEmpty(str))
				return null;

			if (str.StartsWith("."))
				return new List<IEvent> {new SimpleEvent(EventTypes.SendCode, user, str.Substring(1), replaceMsg)};

			if (str.Contains(" "))
				return null;

			foreach (Match match in Regex.Matches(str, @"\w+"))
			{
				if (match.Value.Length != str.Length)
					return null;

				foreach (Match match2 in Regex.Matches(str, @"\d+"))
				{
					if (match.Value.Length == match2.Value.Length)
						return null;

					return new List<IEvent> {new SimpleEvent(EventTypes.SendCode, user, match.Value, replaceMsg)};
				}
			}
			return null;
		}

		public void Refresh()
		{
			var page = _webValidator.GetNextPage();
			_pageController.SetNewPage(page);
		}

		public void SendMsg(IEnumerable<CommandMessage> msgs)
		{
			SendMsgs?.Invoke(msgs);
		}

	}
}
 
