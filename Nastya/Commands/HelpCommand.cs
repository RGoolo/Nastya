using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using Model.BotTypes.Attribute;
using Model.BotTypes.Class;
using Model.BotTypes.Enums;
using Model.Logic.Settings;

namespace Nastya.Commands
{

	[CommandClass(nameof(HelpCommand), "Помощь:", TypeUser.User)]
	public class HelpCommand
	{
		private CommandAttribute GetCommandAttr(MemberInfo mInfo) => mInfo.GetCustomAttribute<CommandAttribute>(true);

		[Command(nameof(Start), "Стартовые данные.", TypeUser.User)]
		public IMessageToBot Start()
		{
			var sb = new StringBuilder();
			sb.AppendLine($"Для получения полной информации: /{nameof(Help)}.");
			
			return MessageToBot.GetTextMsg(sb.ToString());
		}

		[Command(nameof(Help), "Что я умею.", TypeUser.User)]
		public TransactionCommandMessage Help(string classAlias = null)
		{
			var result = new  List<IMessageToBot>();
			//классы с атрибутом CommandClassAttribute
			var refCommands = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.GetCustomAttribute<CommandClassAttribute>(true) != null).ToList();

			//var commands = new List<object>();
			//var property = new Dictionary<string, string>();
			var sb = new StringBuilder();
			if (string.IsNullOrEmpty(classAlias))
			{
				sb.Append("Вас приветствует бот, для игр дозора и дедлайна. Что я умею:\n");
				result.Add(MessageToBot.GetTextMsg(sb.ToString()));
				sb.Clear();

				foreach (var refCommand in refCommands)
					result.AddRange(GetClassInfo(refCommand));
			}
			else
			{
				var @class = refCommands.FirstOrDefault(x =>  x.GetCustomAttribute<CommandClassAttribute>(true).Alias.Equals(classAlias,StringComparison.CurrentCultureIgnoreCase));
				if (@class != null)
					result.AddRange(GetClassInfo(@class));
			}

			//ToDo:
			//property.Join
			//property.Aggregate((current, next) => ($"{current.Key}:{current.Value}", $"{current.Key}:{current.Value}"));
			//property.Join(x => x.Key, x=> x.Key)
			return new TransactionCommandMessage(result);
		}

		private List<IMessageToBot> GetClassInfo(Type refCommand)
		{
			var onlyAdminText = "Только для администраторов группы";

			var sb = new StringBuilder();
			var result = new List<IMessageToBot>();
			var classAttr = refCommand.GetCustomAttribute<CommandClassAttribute>(true);
			if (classAttr != null)
			{
				sb.Append($"Модуль: {classAttr.Description}  /{nameof(Help)}_{classAttr.Alias}");
				if ((classAttr.TypeUser & TypeUser.Admin) == TypeUser.Admin)
					sb.Append($": {onlyAdminText}");
				if (classAttr.TypeUser == TypeUser.Developer)
					sb.Append($": Developer");
				sb.AppendLine();
			}

			var settings = refCommand.GetProperties().Where(x => x.GetCustomAttribute<CommandAttribute>(true) != null).ToList();
			if (settings.Any())
				sb.AppendLine($"Настройки:");
			foreach (var props in settings)
			{
				var attr = props.GetCustomAttribute<CommandAttribute>(true);
				//var attrBr = props.GetCustomAttribute<Browser>(true);
				//if (classAttr.OnlyAdmin || attr.OnlyAdmin)
				//	sb.Append($": {onlyAdminText}");
				if (props.PropertyType == typeof(bool))
				{
					sb.AppendLine($"/{attr.Alias}_on /{attr.Alias}_off");
					sb.AppendLine($"Вкл / Выкл: {attr.Description}");
				}
				else
					sb.Append($"/{attr.Alias} : {attr.Description}");

				sb.Append(Environment.NewLine);
			}
			var infos = refCommand.GetMethods().Where(x => x.GetCustomAttribute<CommandAttribute>(true) != null).ToList();
			if (infos.Any())
				sb.AppendLine($"Функции:");

			foreach (var methodInfo in infos)
			{
				var attr = methodInfo.GetCustomAttribute<CommandAttribute>(true);
				sb.Append($"/{attr.Alias} : {attr.Description}");

				//if (classAttr.OnlyAdmin || attr.OnlyAdmin)
				//	sb.Append($": {onlyAdminText}");

				//if (methodInfo.GetCustomAttribute<CheckPropertyAttribute>(true))
				//	sb.Append($" - {attr2.BoolPropertyName}");

				sb.AppendLine();
			}
			result.Add(MessageToBot.GetTextMsg(sb.ToString()));

			foreach(var custmAttr in refCommand.GetCustomAttributes<CustomHelpAttribute>(true))
				result.Add(MessageToBot.GetTextMsg(custmAttr.CustomHelp));
			
			
			return result;
		}

		private bool IsValid(MethodInfo mi)
		{
			//toDo check
			return true;
		}
	}
}
