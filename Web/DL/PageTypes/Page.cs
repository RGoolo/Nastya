using System.Collections.Generic;
using HtmlAgilityPack;
using Web.Base;
using System.Text.RegularExpressions;
using System.Linq;
using System;
using System.Text;
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
		/// <summary>
		/// Сколько времени до конца задания.
		/// </summary>
		public DateTime? TimeToEnd { get; set; }

		/// <summary>
		/// Заголовок задания.
		/// </summary>
		public string LevelTitle { get; set; }

		/// <summary>
		/// Текст задания
		/// </summary>
		public string Task { get; set; }
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
		public List<Bonus> Bonuses { get; set; } = new List<Bonus>();
		public List<Hint> Hints { get; set; } = new List<Hint>();
		public List<Link> Levels { get; set; } = new List<Link>();

		public TypePage Type { get; set; }
	}
}