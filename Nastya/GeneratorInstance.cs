using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using BotModel.Bots.BotTypes.Attribute;
using BotModel.Bots.BotTypes.Interfaces.Ids;
using BotModel.Settings;
using BotService.Types;
using Model;
using Model.Settings;

namespace NightGameBot
{
    class GeneratorInstance
    {
        public static List<Func<IChatId, ISendMessages, object>> GetInstances()
        {
            var allClasses = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.GetCustomAttribute<CommandClassAttribute>(true) != null).ToList();

            return allClasses.Select(instance => (Func<IChatId, ISendMessages, object>) ((chatId, messages) => CreateInstance(instance, messages, chatId))).ToList();
        }

        private static object CreateInstance(Type type, ISendMessages messages, IChatId chatId)
        {
            //ToDo FirstOrDefault?
            var settings = SettingsHelper.GetChatService(chatId);

            var ctors = type.GetConstructors();
            var ctor = ctors.First();
            var param = new List<object>();

            foreach (var parameterInfo in ctor.GetParameters())
            {
                if (parameterInfo.ParameterType == typeof(IChatId))
                    param.Add(chatId);
                else if (parameterInfo.ParameterType == typeof(IChatService))
                    param.Add(settings);
                else if (parameterInfo.ParameterType == typeof(IChatService0))
                    param.Add(settings);
                else if (parameterInfo.ParameterType == typeof(ISendMessages))
                    param.Add(messages);
            }

            return Activator.CreateInstance(type, param.ToArray());
        }
	}
}
