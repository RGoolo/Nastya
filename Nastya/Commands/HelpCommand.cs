using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Model.Logic.Settings;
using Model.Types.Attribute;
using Model.Types.Class;
using Model.Types.Enums;

namespace Nastya.Commands
{

	[CommandClass(nameof(HelpCommand), "Помощь:", Model.Types.Enums.TypeUser.User)]
	public class HelpCommand
	{
		private CommandAttribute GetCommandAttr(MemberInfo mInfo) => mInfo.GetCustomAttribute<CommandAttribute>(true);

		[Command(nameof(Start), "Стартовые данные.", Model.Types.Enums.TypeUser.User)]
		public List<CommandMessage> Start()
		{
			var result = new List<CommandMessage>();
			
			var sb = new StringBuilder();
			sb.AppendLine($"Добрый день. Вас приветствует бот для ночных игр. Для полной информации введите: /{nameof(Help)}.");
			sb.AppendLine("При первом запуске заполните данные аккаунта движка, из под которых будет играть бот:");

			result.Add(CommandMessage.GetTextMsg(sb.ToString()));
			sb.Clear();

			sb.AppendLine($"/{Const.Game.Uri} http://classic.dzzzr.ru/demo/");
			sb.AppendLine($"/{Const.Game.Login} login");
			sb.AppendLine($"/{Const.Game.Password} \"password\"");

			result.Add(CommandMessage.GetTextMsg(sb.ToString()));
			return result;
		}

		[Command(nameof(Help), "Что я умею.", Model.Types.Enums.TypeUser.User)]
		public TransactionCommandMessage Help(string classAlias)
		{
			var result = new  List<CommandMessage>();
			//классы с атрибутом CommandClassAttribute
			var refCommands = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.GetCustomAttribute<CommandClassAttribute>(true) != null).ToList();

			//var commands = new List<object>();
			//var property = new Dictionary<string, string>();
			var sb = new StringBuilder();
			if (string.IsNullOrEmpty(classAlias))
			{
				sb.Append("Вас приветствует бот, для игр дозора и дедлайна. Что я умею:\n");
				result.Add(CommandMessage.GetTextMsg(sb.ToString()));
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

		private List<CommandMessage> GetClassInfo(Type refCommand)
		{
			var onlyAdminText = "Только для администраторов группы";

			var sb = new StringBuilder();
			var result = new List<CommandMessage>();
			var classAttr = refCommand.GetCustomAttribute<CommandClassAttribute>(true);
			if (classAttr != null)
			{
				sb.Append($"Модуль: {classAttr.Description}");
				if ((classAttr.TypeUser & TypeUser.Admin) == TypeUser.Admin)
					sb.Append($": {onlyAdminText}");
				if (classAttr.TypeUser == TypeUser.Developer)
					sb.Append($": Developer");
				sb.Append($"Модуль: {classAttr.Description} /{nameof(Help)}_{classAttr.Alias} ");
				sb.Append("\n");
			}

			var settings = refCommand.GetProperties().Where(x => x.GetCustomAttribute<CommandAttribute>(true) != null).ToList();
			if (settings.Any())
				sb.Append($"Настройки:\n");
			foreach (var props in settings)
			{
				var attr = props.GetCustomAttribute<CommandAttribute>(true);
				//var attrBr = props.GetCustomAttribute<Browser>(true);
				//if (classAttr.OnlyAdmin || attr.OnlyAdmin)
				//	sb.Append($": {onlyAdminText}");
				if (props.PropertyType == typeof(bool))
				{
					sb.Append($"/{attr.Alias}_on /{attr.Alias}_off\n");
					sb.Append($"Вкл / Выкл: {attr.Description}\n");
				}
				else
					sb.Append($"/{attr.Alias} : {attr.Description}");

				sb.Append(Environment.NewLine);
			}
			var infos = refCommand.GetMethods().Where(x => x.GetCustomAttribute<CommandAttribute>(true) != null).ToList();
			if (infos.Any())
				sb.Append($"Функции:\n");

			foreach (var methodInfo in infos)
			{
				var attr = methodInfo.GetCustomAttribute<CommandAttribute>(true);
				sb.Append($"/{attr.Alias} : {attr.Description}");

				//if (classAttr.OnlyAdmin || attr.OnlyAdmin)
				//	sb.Append($": {onlyAdminText}");

				//if (methodInfo.GetCustomAttribute<CheckPropertyAttribute>(true))
				//	sb.Append($" - {attr2.BoolPropertyName}");

				sb.Append("\n");
			}
			result.Add(CommandMessage.GetTextMsg(sb.ToString()));

			foreach(var custmAttr in refCommand.GetCustomAttributes<CustomHelpAttribute>(true))
				result.Add(CommandMessage.GetTextMsg(custmAttr.CustomHelp));
			
			
			return result;
		}

		private bool IsValid(MethodInfo mi)
		{
			//toDo check
			return true;
		}
	}
}
