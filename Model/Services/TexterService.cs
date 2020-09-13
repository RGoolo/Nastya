using System;
using System.Net;
using BotModel.Bots.BotTypes.Class;
using BotModel.Bots.BotTypes.Interfaces.Ids;
using BotModel.Bots.TelegramBot.HtmlParse;
using BotModel.Bots.TelegramBot.Services;
using BotModel.Logger;
using BotModel.Settings;
using Model.Settings;

namespace Model.Services
{
    public class TexterService : ITexterService
    {
		public Texter GetNormalizeText(Texter text, IChatId chatId)
		{
			if (text?.Html != true)
				return text;

			try
			{
				if (text.ReplaceCoordinates)
				{
					var set = SettingsHelper.GetChatService(chatId);
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
					Logger.CreateLogger(nameof(CookieContainer)).Warning(e);
				}
			}

			return text.Replace(TelegramHtml.RemoveAllTag(text?.Text), false);
		}
	}
}