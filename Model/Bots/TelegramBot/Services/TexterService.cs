using System;
using System.Net;
using Model.Bots.BotTypes.Class;
using Model.Bots.BotTypes.Interfaces.Ids;
using Model.Bots.TelegramBot.HtmlParse;
using Model.Logic.Settings;

namespace Model.Bots.TelegramBot.Services
{
	public class TexterService
	{
		public static Texter GetNormalizeText(Texter text, IChatId chatId)
		{
			if (text?.Html != true)
				return text;

			try
			{
				if (text.ReplaceCoordinates)
				{
					var set = SettingsHelper.GetSetting(chatId);
					var points = set.PointsFactory.GetCoordinates(text.Text);
					text.Replace(points.ReplacePoints(), true);
				}

				var t = TelegramHtml.RemoveTag(text); //, GetParseMod(text));
				if (TelegramHtml.CheckPaarTags(t))
					return text.Replace(t, true);
			}
			catch (Exception e)
			{
				{
					Logger.Logger.CreateLogger(nameof(CookieContainer)).Warning(e);
				}
			}

			return text.Replace(TelegramHtml.RemoveAllTag(text?.Text), false);
		}
	}
}