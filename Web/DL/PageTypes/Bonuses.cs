using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace Web.DL.PageTypes
{
	public class Bonuses 
	{
		public Bonuses(IEnumerable<HtmlNode> nodes)
		{
			if (nodes == null) return;
			_bonuses = nodes.Select(n => new Bonus(n)).ToList();
		}

		private readonly List<Bonus> _bonuses = new List<Bonus>();
		public int CountReady => _bonuses.Count(x => x.IsReady);
		public int Count => _bonuses.Count(x => x.IsReady);
		public void Add(Bonus bonus) => _bonuses.Add(bonus);
		public IEnumerable<Bonus> AllBonuses => _bonuses;
		public IEnumerable<Bonus> ReadyBonuses => _bonuses.Where(x => x.IsReady);

		public bool IsEmpty => _bonuses.Count == 0;
	}
}
