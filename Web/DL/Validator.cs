using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model.Logic.Model;
using Model.Logic.Settings;
using Model.Types.Class;
using Web.Base;
using Web.Game.Model;

namespace Web.DL
{
	public class Validator : BaseValidator
	{
		private Page _lastPage;
		const string TimeFormat = "HH:mm:ss";

		public Validator(ISettings setting) : base(setting)
		{

		}

		public override void AfterSendCode(string html, string code, Guid? idMsg)
		{
			var page = new Page(html);
			if (page?.LevelId == null || !page.IsReceived.HasValue)
				throw new GameException("Сервер вернул некорректное значение.");

			SendTexttMsg(page.IsReceived.Value ? "+" : "-" + " " + (code), idMsg);

			SetNewPage(page);
		}

		public void SetNewPage(Page page)
		{
			//if (Settings.GetValueBool(Game.))
			if (page?.LevelId == null)
				throw new GameException("Сервер вернул некорректное значение.");

			var lvlNumber = Settings.Page.LastLvl;
			if (_lastPage == null)
			{
				_lastPage = page;
				if (lvlNumber != page.LevelNumber)
					SendNewLevelInfo(page, true);
			}
			else
			{
				if (_lastPage.LevelNumber != page.LevelNumber)
					SendNewLevelInfo(page, true);
				else
					SendDiffTime(page);
			}

			_lastPage = page;
			if (lvlNumber != _lastPage.LevelNumber)
				Settings.SetValue(Const.Page.LastLvl, _lastPage.LevelNumber);
		}

		private bool IsBorderValue(DateTime dt1, DateTime dt2, int minutes)
		{
			DateTime maxDt;
			DateTime minDt;
			if (dt1 > dt2)
			{
				maxDt = dt1;
				minDt = dt2;
			}
			else
			{
				maxDt = dt2;
				minDt = dt1;
			}
			
			if (maxDt.Minute > minutes && maxDt.Hour == 0)
				if (minDt.Minute < minutes)
					return true;

			return false;
		}

		private void SendDiffTime(Page page)
		{
			var msg = new List<CommandMessage>();
	
			if (IsBorderValue (page.TimeToEnd , _lastPage.TimeToEnd, 5))
				msg.Add(new Text($"! Осталось меньше 5 минут"));

			if (IsBorderValue(page.TimeToEnd, _lastPage.TimeToEnd, 1))
				msg.Add(new Text($"! Осталось меньше минуты"));


			for (var i = 0; i < page.Hints.Count; ++i)
			{
				if (!page.Hints[i].IsReady)
				{
					var hint = _lastPage.Hints.Where(x => x.Number == page.Hints[i].Number && page.Hints[i].Number != 0).FirstOrDefault();
					if (hint == null)
						continue;

					if (IsBorderValue(page.Hints[i].TimeTo, hint.TimeTo, 5))
						msg.Add(new Text($"{hint.Name} Откроется через {hint.TimeTo.ToString(TimeFormat)}"));

					if (IsBorderValue(page.Hints[i].TimeTo, hint.TimeTo, 1))
						msg.Add(new Text($"{hint.Name} Откроется через {hint.TimeTo.ToString(TimeFormat)}"));
		}
			}
			if (msg.Any())
				SndMsg(msg);

		}

		public override void SetNewPage(string html)
		{
			SetNewPage(new Page(html));
		}

		public override string LogInContext() =>
			$@"socialAssign=0&Login={Login()}&Password={Password()}&EnButton1=Sign+In&ddlNetwork=1";

		public override string GetUrl()
		{
			var result = $@"{Site()}{Settings.Web.BodyRequest}/{Settings.Web.GameNumber}/";

			if (Settings.Game.Sturm)
			{
				var lvl = Settings.Game.Level;
				if (!string.IsNullOrEmpty(lvl))
					return result + "?level=" + lvl;
			}
			return result;
		}

		public override string LogInUrl() => $@"{Site()}Login.aspx";

		public override string GetContextSetCode(string code)
		{
			if (_lastPage?.LevelId == null)
				throw new GameException("Сервер вернул некорректное значение.");
			return $"LevelId={_lastPage.LevelId}&LevelNumber={_lastPage.LevelNumber}&LevelAction.Answer=" + code;
		}

		private string Domen() => Settings.Web.Domen;
		private string Site() => $@"http://{Domen()}/";
		private string Login() => Settings.Game.Login;
		private string Password() => Settings.Game.Password;

		public void SendNewLevelInfo(Page page, bool isNewlvl = false)
		{
			var msg = new List<CommandMessage>();

			StringBuilder sb = new StringBuilder();
			sb.Append(!isNewlvl ? "❤️ Текущий уровень ❤️\n" : "❤️ Следующий уровень ❤️\n");

			if (page.Levels.Any())
			{
				sb.Append("Уровни: \n");
				page.Levels.ForEach(x => sb.Append(x + "		"));
				sb.Append("\n");
			}

			if (page.TimeToEnd != default(DateTime))
				sb.Append($"Времени для автоперехода: " + page.TimeToEnd.ToString(TimeFormat) + "\n");

			sb.Append(page.LevelTitle + "\n" + page.Task + "\n");
			
			if (page.Links.Count > 0)
			{
				sb.Append("В задании есть следующие ссылки: \n");
				page.Links.ForEach(x => sb.Append(x + "\n"));
			}

			if (!string.IsNullOrEmpty(page.Sectors.SectorsRemain) )
			{
				sb.Append($"На уровне осталось закрыть: {page.Sectors.SectorsRemain}(/sectors) из {page.Sectors.CountSectors}(/allsectors).\n");
			}

			if (page.Bonuses.Any())
			{
				var isReady = page.Bonuses.Where(x => x.IsReady).Count();
				sb.Append($"на уровне закрыто {isReady}(/{Const.Game.Bonus}) из {page.Bonuses.Count()}(/{Const.Game.AllBonus})\n");
				//page.Bonuses.ForEach(x => sb.Append(x.IsReady + x.Name + "\n" + x.Text + "\n"));
				//sb.Append($"На уровне осталось закрыть: {page.Sectors.SectorsRemain}(/sectors) из {page.Sectors.CountSectors}(/allsectors).\n");
			}

			if (page.Hints.Any())
			{
				foreach (var hint in page.Hints)
				{
					sb.Append(hint.IsReady
						? $"\n{hint.Name}: {hint.Text}\n"
						: $"\n{hint.Name} откроется через: {hint.TimeTo.ToString(TimeFormat)}\n");
				}

				//sb.Append($"На уровне осталось закрыть: {page.Sectors.SectorsRemain}(/sectors) из {page.Sectors.CountSectors}(/allsectors).\n");
			}

			var textTask = WebHelper.RemoveImg(WebHelper.RemoteTagToTelegram(sb.ToString()));

			var text = textTask.Item1 + "\n";
			foreach (var img in textTask.Item2)
			{
				text = text.Replace(img.Name, $"<a href=\"{img.Url}\">{img.Name}</a>");
			}

			text = text.Replace("<a>", "</a>)");
			 
			msg.Add(new Text(text, true, withHtml: true));


			var currentCoords = Model.Logic.Coordinates.Coordinates.GetCoords(sb.ToString()).ToList();
			foreach (var x in currentCoords)
				msg.Add(new MessageCoord(x));

			if (page.ImageUrls.Any())
			{
				if (page.ImageUrls.Count > 10)
					msg.Add(new Text("Тут должны быть картинки, но их больше 10, так что не загружаю!" , true));
				else
					msg.AddRange(page.ImageUrls.Select(x => new Photo(x, true,parameter: x)));
			}
			SndMsg(msg);
		}

		public void SendBonus(Page page, bool isAll = false)
		{
			var msg = new List<CommandMessage>();
			StringBuilder sb = new StringBuilder("");

			if (!page.Bonuses.Any())
				sb.Append("Нет бонусов");
			else
				page.Bonuses.Where(x => isAll || !x.IsReady).ToList().ForEach(x => sb.Append(x.Name + "\n" +( string.IsNullOrEmpty(x.Text) ? ("\n") : (x.Text + "\n\n"))));
			
			msg.Add(new Text(sb.ToString() == ""?"Все бонусы закрыты.": sb.ToString(), true));
			SndMsg(msg);
		}

		public void SendSectors(Page page, bool isAll = false)
		{
			var msg = new List<CommandMessage>();
			StringBuilder sb = new StringBuilder();

			if ((page.Sectors.SectorsRemain ?? "") != "")
			{
				var sectors = page.Sectors.Sectors.Where(x => (!x.Accepted || isAll));
				sb.Append($"На уровне осталось закрыть: {page.Sectors.SectorsRemain} из {page.Sectors.CountSectors}\n");
				sectors.ToList().ForEach(x =>
					sb.Append(
						$"{(x.Accepted ? "" : "<b>")}{x.Name} : {(x.Accepted ? x.Answer : "-")}{(x.Accepted ? "" : "</b>")}\n"));
			}
			else
			{
				sb.Append($"На уровне нет секторов");
			}

			msg.Add(new Text(sb.ToString(), true, withHtml: true));
			SndMsg(msg);
		}

		public override void SendEvent(IEvent iEvent)
		{

			switch (iEvent.EventType)
			{
				case EventTypes.Refresh:
				case EventTypes.SendCode:
				case EventTypes.StartGame:
				case EventTypes.StopGame:
					break;

				case EventTypes.GetLvlInfo:
				case EventTypes.GetAllInfo:
					SendNewLevelInfo(_lastPage);
					break;
				case EventTypes.GetBonus:
					SendBonus(_lastPage, false);
					break;
				case EventTypes.GetAllBonus:
					SendBonus(_lastPage, true);
					break;

				case EventTypes.GetAllSectors:
					SendSectors(_lastPage);
					break;
				case EventTypes.GetSectors:
					SendSectors(_lastPage, true);
					break;

				case EventTypes.GetTimeForEnd:
					SendTexttMsg($"Времени до автоперехода: {_lastPage.TimeToEnd.ToString(TimeFormat)}");
					break;
			}
		}
	}
}
 
