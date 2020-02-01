using System;
using System.Collections.Generic;
using Model.BotTypes.Interfaces;
using Model.BotTypes.Interfaces.Messages;
using Model.Files.FileTokens;


namespace Model.BotTypes.Class.Reflection
{
	public static class CustomStructureMapper
	{
		private static Dictionary<Type, Func<IBotMessage, IMessageCommand, object>> parameters => new Dictionary<Type, Func<IBotMessage, IMessageCommand, object>>()
		{
			[typeof(string[])] = (mess, command) => command.Values.ToArray(),
			[typeof(IChatFile)] = (mess, command) => mess.Resource.File,
			[typeof(IChatFile)] = (mess, command) => mess.Resource.File,
			
			[typeof(IEnumerable<string>)] = (mess, command) => command.Values,
			[typeof(IMessageCommand)] = (mess, command) => command,
			[typeof(IBotMessage)] = (mess, command) => mess,
			[typeof(IUser)] = (mess, command) => mess.User,
		};
	}
}