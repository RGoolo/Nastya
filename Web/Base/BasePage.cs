using System;
using System.Collections.Generic;

namespace Web.Base
{
/*
    public abstract class BasePage
    {

		/// <summary>
		/// Сколько времени до конца задания.
		/// </summary>
		public DateTime TimeToEnd;

        /// <summary>
        /// Заголовок задания.
        /// </summary>
        public string LevelTitle;
        /// <summary>
        /// Текст задания
        /// </summary>
        public string Task;
        /// <summary>
        /// Ссылки на картинки из текста задания.
        /// </summary>
        public List<string> ImageUrls;
        /// <summary>
        /// Прочие ссылки из текста задания
        /// </summary>
        public List<ILink> Links;
        /// <summary>
        /// Порядковый номер задания
        /// </summary>
        public string LevelNumber;

        /// <summary>
        /// Код принят
        /// false: Неверный код
        //  true:  Верный код
        /// null:  Код не вводился
        /// </summary>
        public bool? IsReceived;

        //public virtual void SetHTML(string html);

        public SectorsCollection Sectors;

    }
*/
    public class Sector
    {
        public string Name;
        public string Answer;
        public bool Accepted;
    }

    public class SectorsCollection
    {
        public List<Sector> Sectors;
        /// <summary>
        ///  Сколько всего секторов
        /// </summary>
        public string CountSectors;
        /// <summary>
        /// Сколько секторов осталось закрыть
        /// </summary>
        public string SectorsRemain;

        public SectorsCollection()
        {
            Sectors = new List<Sector>();
        }
    }

    public interface ILink
    {
        string Name { get; }
        string Url { get; }
    }
}
