using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Model.BotTypes.Class;
using Model.BotTypes.Interfaces;
using Model.BotTypes.Interfaces.Messages;
using Model.Logic.Settings;
using Web.Base;
using Web.DL.PageTypes;
using Web.Game.Model;

namespace Web.DL
{

	public interface ISendMsgDl
	{
		void SendMsg(IEnumerable<IMessageToBot> msgs);
	}

	public class DlController : IController, ISendMsgDl
	{
		public event SendMsgsSyncDel SendMsgs;

		public ISettings Setting { get; }

		private readonly IDlValidator _webValidator;

		public ISettings Settings => Setting;

		private PageController _pageController;

		public DlController(ISettings setting)// : base(setting)
		{
			Setting = setting;
			_webValidator = FactoryValidator.CreateValidator(setting);
			_pageController = new PageController(this, setting);
		}

		public void SendEvent(IEvent iEvent)
		{
			switch (iEvent.EventType)
			{
				case EventTypes.GetLvlInfo:
				case EventTypes.GetAllInfo:
					_pageController.SendNewLevelInfo(_pageController.GetCurrentPage);
					break;
				case EventTypes.GetBonus:
					_pageController.SendBonus(_pageController.GetCurrentPage, false);
					break;
				case EventTypes.GetAllBonus:
					_pageController.SendBonus(_pageController.GetCurrentPage, true);
					break;

				case EventTypes.GetAllSectors:
					_pageController.SendSectors(_pageController.GetCurrentPage);
					break;
				case EventTypes.GetSectors:
					_pageController.SendSectors(_pageController.GetCurrentPage, true);
					break;

				case EventTypes.GetTimeForEnd:
					_pageController.SendMsg($"Времени до автоперехода: {_pageController.GetCurrentPage.TimeToEnd?.ToString()}");
					break;

				case EventTypes.SendCode:
					_pageController.AfterSendCode(_webValidator.SendCode(iEvent.Text, _pageController.GetCurrentPage).Result, iEvent); //ToDo: delete Result
					break;

				case EventTypes.SendSpoiler:
				case EventTypes.TakeBreak:
				case EventTypes.GoToTheNextLevel:
				default:
					throw new NotImplementedException("Не реализовал пока.");
			}
		}

		public void LogIn() => _pageController.SetNewPage(_webValidator.LogIn().Result); //ToDo delete Result

		public List<IEvent> GetCode(string str, IUser user, IMessageId replaceMsg)
		{
			if (string.IsNullOrEmpty(str))
				return null;

			if (str.StartsWith("."))
				return new List<IEvent> {new SimpleEvent(EventTypes.SendCode, user, str.Substring(1), replaceMsg)};

			const string symbol = "[a-zA-Zа-яА-ЯёЁ]";
			var  match = Regex.Match(str, $@"^({symbol}+\d\w*)|(\d+{symbol}\w*)$");

			return match.Success ? new List<IEvent> {new SimpleEvent(EventTypes.SendCode, user, match.Value, replaceMsg)} : null;
		}

		public void Refresh()
		{
			var page = _webValidator.GetNextPage().Result; //ToDo: delete Result;
			_pageController.SetNewPage(page);
		}

		public void SendMsg(IEnumerable<IMessageToBot> msgs)
		{
			SendMsgs?.Invoke(msgs);
		}
	}
}
 