using System.Collections.Generic;
using System.Text;
using Model.BotTypes.Class;
using Model.Logic.Settings;
using Web.Base;

namespace Web.DZR
{
	public static class CheckChanges
	{
		public static Texter GetTaskInfo(DzrTask task, bool newTask, string timeForEnd)
		{
			StringBuilder taskText = new StringBuilder();

			if (newTask)
				taskText.AppendLine("📩Новое Задание");

			taskText.AppendLine(task.TitleText);

			if (!string.IsNullOrEmpty(timeForEnd))
				taskText.Append(timeForEnd);

			taskText.AppendLine(task.Text);

			if (task.Spoilers != null)
				taskText.Append(task.Spoilers.Text());

			foreach (var hint in task._hints)
				taskText.AppendLine($"📖{hint.Name}\n{hint.Text}");

			taskText.AppendLine($"Коды сложности /{Const.Game.Codes} остались /{Const.Game.LastCodes}:");

			foreach (var codes in task.Codes)
				taskText.AppendLine(codes.Text());

			return new Texter(taskText.ToString(), true);
		}
	}
}