using System.Collections.Generic;
using System.Text;
using BotModel.Bots.BotTypes.Attribute;
using BotModel.Bots.BotTypes.Class;
using BotModel.Bots.BotTypes.Enums;
using BotModel.Bots.BotTypes.Interfaces.Ids;
using BotModel.Bots.BotTypes.Interfaces.Messages;
using BotModel.Files.FileTokens;
using BotModel.Settings;
using Model;
using Model.Settings;


namespace NightGameBot.Commands
{
    
    [CommandClass("NumberOfPicture", "Добавляет подпись к картинке.", TypeUser.User)]
    class NumberOfPicture
    {
        [Command(nameof(AddNumber), "Реагировать на комманды без префикса бота.")]
        public bool AddNumber { get; set; }

        [Command(nameof(PictureNumber), "Реагировать на комманды без префикса бота.")]
        public int PictureNumber { get; set; }

        [CommandOnMsg(nameof(AddNumber), MessageType.Photo, typeUser: TypeUser.User)]
        public IMessageToBot AddPicture(IBotMessage msg, ISettingsPage settings)
        {
            PictureNumber++;
            var result = MessageToBot.GetTextMsg(Desc(settings.LastLvl, PictureNumber));
            result.OnIdMessage = msg.MessageId;
            return result;
        }

        private string Desc(string lvl, int number) => $"{lvl}L{number}";

    }
}
