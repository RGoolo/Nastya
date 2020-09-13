using System.IO;
using BotModel.Bots.BotTypes.Class.Ids;
using BotModel.Bots.BotTypes.Interfaces.Ids;
using BotModel.Bots.BotTypes.Interfaces.Messages;
using BotModel.Settings;
using HtmlAgilityPack;
using Model.Settings;

namespace Model
{
    public static class SettingsHelper
    {
        public static IChatService GetChatService(IUser user)
        {
            return GetChatService(new ChatGuid(user.Id));
        }

        public static IChatService GetChatService(IChatId chatId)
        {
            return new SettingHelper(SettingsHelper0.GetChatService0(chatId));
        }
    }
}