using System;
using System.Linq;
using System.Text;
using BotModel.Bots.BotTypes;
using BotModel.Bots.BotTypes.Enums;
using BotModel.Exception;

namespace Model.Settings
{
	public static class UrlService
	{
		private static bool StartWith(StringBuilder sb, string str, bool replace = false)
		{
			if (string.IsNullOrEmpty(str))
				return true;
			if (sb.Length < str.Length)
				return false;
			if (sb.ToString().StartsWith(str, StringComparison.CurrentCultureIgnoreCase))
			{
				if (replace)
					sb.Remove(0, str.Length);
				return true;

			}
			return false;
		}

		public static string GetDefaultUrl(IChatService settings, TypeGame result)
		{
			var url = settings.Web.Domen;
			if (result == TypeGame.Unknown) return string.Empty;

			if (result.IsDummy())
				return url.Replace('/', '\\').Split('\\').SkipLast(1).Aggregate((x, y) => x + "\\" + y); //ToDo wtf?
			

			if ((result & TypeGame.Dzzzr) == TypeGame.Dzzzr)
				return $@"http://{settings.Web.Domen}/{settings.Web.BodyRequest}/go/";

			return string.Empty;
		}

		public static TypeGame SetUri(IChatService settings, string uri)
		{
			var dummy = "dummy:";
			var lite = "lite:";
			var dzzzr = "dzzzr:";
			var deadLine = "dl:";
			var prequel = "pr:";

			var result = TypeGame.Unknown;
			if (string.IsNullOrEmpty(uri))
				return result;

			StringBuilder newUri = new StringBuilder(uri);
			settings.SetValue(Const.Web.Domen, newUri.ToString());

			if (StartWith(newUri, dummy, true))
			{
				result |= TypeGame.Dummy;
				if (StartWith(newUri, prequel, true))
					result |= TypeGame.Prequel;

				if (StartWith(newUri, lite, true))
					result |= TypeGame.Lite;
				else if (StartWith(newUri, dzzzr, true))
					result |= TypeGame.Dzzzr;
				else if (StartWith(newUri, deadLine, true))
					result |= TypeGame.DeadLine;

				settings.SetValue(Const.Web.Domen, newUri.ToString());
				//Settings.SetValue(Const.Game.Uri, newUri.ToString());
				return result;
			}

			if (StartWith(newUri, "https://", true))
			{
				//empty
			}

			if (StartWith(newUri, "http://", true))
			{
				//empty
			}

			if (StartWith(newUri, "lite.dzzzr.ru", false))
			{
				result |= TypeGame.Lite;

				var site = newUri.ToString().Split('/');
				settings.SetValue(Const.Web.Domen, site[0]);
				settings.SetValue(Const.Web.BodyRequest, site[1]);

				if (uri.Contains("?pin="))
				{
					settings.SetValue(Const.Web.GameNumber, int.Parse(site[3].Replace("?pin=", "")).ToString());
				}
				else
				{
					result |= TypeGame.Prequel;
					settings.SetValue(Const.Web.GameNumber, "0");
				}

				return result;
			}

			if (StartWith(newUri, "classic.dzzzr.ru", false))
			{
				//classic.dzzzr.ru/spb/go/
				result |= TypeGame.Dzzzr;
				var site = newUri.ToString().Split('/');
				settings.SetValue(Const.Web.BodyRequest, site[1]);
				settings.SetValue(Const.Web.Domen, site[0]);

				if (uri.Contains("section=anons"))
					result |= TypeGame.Prequel;

				return result;
			}

			//demo.en.cx/GameDetails.aspx?gid=26569
			result |= TypeGame.DeadLine;
			if (uri.Contains("GameDetails"))
			{
				settings.SetValue(Const.Web.GameNumber, newUri.ToString().Split("=")[1]);
				settings.SetValue(Const.Web.Domen, newUri.ToString().Split("/")[0]);
				settings.SetValue(Const.Web.BodyRequest, "gameengines/encounter/play");
				return result;
			}

			//demo.en.cx/gameengines/encounter/play/26569
			var correct = newUri.ToString().Split("/", StringSplitOptions.RemoveEmptyEntries);

			if (correct.Length < 4)
				throw new GameException("Не удалось номер распарить ссылку");

			if (!int.TryParse(correct.Last(), out int numberGame))
				throw new GameException("Не удалось номер игры получить");

			settings.SetValue(Const.Web.GameNumber, numberGame.ToString());
			settings.SetValue(Const.Web.Domen, correct[0]);
			var bodyRequest = correct.Skip(1).SkipLast(1).Aggregate((x, y) => x + "/" + y);
			//for (var i = 1; i < correct.Length - 1; i++)
			//bodyRequest += correct[i] + "/";
			settings.SetValue(Const.Web.BodyRequest, bodyRequest);

			return result;
		}

	}
}