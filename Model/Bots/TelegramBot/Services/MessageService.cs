using System.Collections.Generic;
using Model.Bots.BotTypes;
using Model.Bots.BotTypes.Class;
using Model.Bots.BotTypes.Enums;
using Model.Bots.BotTypes.Interfaces.Ids;
using Model.Bots.TelegramBot.HtmlParse;
using Model.Logic.Settings;

namespace Model.Bots.TelegramBot.Services
{
	public static class MessageService
	{
		public static List<IMessageToBot> ChildrenMessage(IMessageToBot msg, IChatId chatId)
		{
			var result = new List<IMessageToBot>();
			if (msg.Text?.Html != true || (msg.TypeMessage & MessageType.Text) == 0 || string.IsNullOrEmpty(msg.Text.Text))
				return result;

			var setting = SettingsHelper.GetSetting(chatId);
			var defaultUrl = setting.Web.DefaultUri;

			var links = TelegramHtml.GetLinks(msg.Text.Text, defaultUrl);
			if (links.Count == 0 || !msg.Text.ReplaceResources)
				return result;

			var str = TelegramHtml.ReplaceTagsToHref(msg.Text.Text, links);

			msg.Text.Replace(str, true);
			var img = 0;
			foreach (var link in links)
			{
				switch (link.TypeUrl)
				{
					case TypeUrl.Img:
						if (img++ > msg.Text.Settings.MaxParsePicture)
							continue;

						var file = setting.TypeGame.IsDummy()
							? setting.FileChatFactory.GetExistFileByPath(link.Url)
							: setting.FileChatFactory.InternetFile(link.Url);
						result.Add(MessageToBot.GetPhototMsg(file, (Texter)link.Name));
						break;
					case TypeUrl.Sound:
						result.Add(MessageToBot.GetVoiceMsg(link.Url, link.Name));
						break;
				}
			}

			return result;
		}
	}
}