using System.Collections.Generic;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using Model.Bots.BotTypes.Class;
using Model.Logic.Settings;
using Web.DL.PageTypes;

namespace Web.DL
{

	public enum TypeCode
	{
		None = 0,
		Received,
		NotReceived,
	}

	public enum TypePage
	{
		Unknown,
		NotStarted,
		InProcess,
		Finished,
		ErrorAuthentication,
	}

	public class DLPage
	{
		public string Html { get; set; }

		/// <summary>
		/// Сколько времени до конца задания.
		/// </summary>
		public TimeSpan? TimeToEnd { get; set; }

		/// <summary>
		/// Заголовок задания.
		/// </summary>
		public string LevelTitle { get; set; }

		/// <summary>
		/// Текст задания
		/// </summary>
		public string Body { get; set; }
		/// <summary>
		/// Ссылки на картинки из текста задания.
		/// </summary>
		
		//public List<string> ImageUrls { get; set; }
		/// <summary>
		/// Прочие ссылки из текста задания
		/// </summary>
		
		//public List<ILink> Links { get; set; }
		/// <summary>
		/// Порядковый номер задания
		/// </summary>
		public string LevelNumber { get; set; }

		public TypeCode CodeType { get; set; }

		public SectorsCollection Sectors { get; set; }

		/// <summary>
		/// ID задания в движке
		/// </summary>
		public string LevelId { get; set; }
		public Bonuses Bonuses { get; set; } 

		public Hints Hints { get; set; } = new Hints();
		public List<string> Levels { get; set; } = new List<string>();

		public bool IsSturm { get; set; }

		public TypePage Type { get; set; }
	}

	public static class DlPageExtension
	{
		public static Texter ToTexter(this DLPage page, bool newLvl)
		{
			StringBuilder sb = new StringBuilder();

			if (page.Type == TypePage.NotStarted)
			{
				sb.AppendLine(page.Body);
				sb.AppendLine($"⏳ Времени до старта: " + page.TimeToEnd);

				return new Texter(sb.ToString());
			}

			sb.AppendLine((!newLvl ? "📖 Текущий уровень" : $"📖 Новый уровень") + $" #{page.LevelNumber}");

			if (page.Levels.Any())
			{
				sb.AppendLine("Уровни:");
				page.Levels.ForEach(x => sb.Append($"/{Const.Game.Level}_{x}\t"));
				sb.AppendLine();
			}

			if (page.TimeToEnd != default(TimeSpan))
				sb.AppendLine($"⏳ Времени для автоперехода: " + page.TimeToEnd);

			sb.AppendLine(page.LevelTitle);
			sb.AppendLine(page.Body);

			if (!string.IsNullOrEmpty(page.Sectors?.SectorsRemainString))
				sb.AppendLine($"На уровне осталось закрыть секторов: {page.Sectors.SectorsRemainString}(/sectors) из {page.Sectors.CountSectors}(/allsectors).");
			
			if (!page.Bonuses.IsEmpty)
			{
				var isReady = page.Bonuses.CountReady;
				sb.AppendLine($"На уровне осталось закрыть бонусов: {isReady}(/{Const.Game.Bonus}) из {page.Bonuses.Count}(/{Const.Game.AllBonus})");
			}

			if (page.Hints.Any())
				sb.AppendLine(page.Hints.ToString());
			
			return new Texter(sb.ToString(), true);
		}
	}
}