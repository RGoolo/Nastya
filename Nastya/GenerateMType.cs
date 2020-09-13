using System;
using System.Collections.Generic;
using BotModel.Bots.BotTypes.Enums;
using BotModel.Bots.BotTypes.Interfaces.Messages;
using BotModel.Settings;
using Model;
using Model.Logic.Coordinates;
using Model.Settings;

namespace NightGameBot
{
    public class GeneratorTypes
    {
        public static List<(Type, Func<IBotMessage, IMessageCommand, object>)> Generate()
        {
            return new List<(Type, Func<IBotMessage, IMessageCommand, object>)>
            {
                (typeof(IChatService), (mess, command) => SettingsHelper.GetChatService(mess.Chat.Id)),
                (typeof(ISettingsBraille),
                    (mess, command) => SettingsHelper.GetChatService(mess.Chat.Id).Braille),
                (typeof(ISettingsCoordinates),
                    (mess, command) => SettingsHelper.GetChatService(mess.Chat.Id).Coordinates),
                (typeof(IDlSettingsGame),
                    (mess, command) => SettingsHelper.GetChatService(mess.Chat.Id).DlGame),
                (typeof(IDzzzrSettingsGame),
                    (mess, command) => SettingsHelper.GetChatService(mess.Chat.Id).DzzzrGame),
                (typeof(ISettingsGame), (mess, command) => SettingsHelper.GetChatService(mess.Chat.Id).Game),
                (typeof(ISettingsPage), (mess, command) => SettingsHelper.GetChatService(mess.Chat.Id).Page),
                (typeof(IPointsFactory),
                    (mess, command) => SettingsHelper.GetChatService(mess.Chat.Id).PointsFactory),
                (typeof(TypeGame), (mess, command) => SettingsHelper.GetChatService(mess.Chat.Id).TypeGame),
                (typeof(ISettingsWeb), (mess, command) => SettingsHelper.GetChatService(mess.Chat.Id).Web)
            };
        }
    }
}