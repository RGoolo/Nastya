namespace Web.DZR
{
	public class DzrUrl
	{
		private string Nostat { get; set; }
		private string GetNostat => $@"nostat={Nostat}";

		private string notags { get; set; }
		private string GetNotags => $@"notags={notags}";
		private string legend { get; set; }
		private string GetLegend => $@"legend={legend}";
		private string log { get; set; }
		private string GetLog => $@"log={log}";
		private string refresh { get; set; }
		private string GetRefresh => $@"refresh={refresh}";
		private string bonus { get; set; }
		private string GetBonus => $@"bonus={bonus}";
		private string KladMap { get; set; }
		private string GetKladMap => $@"kladMap={KladMap}";

		public override string ToString() => $"{GetNostat}&{GetNotags}&{GetLegend}&{GetLog}&{GetRefresh}&{GetBonus}&{GetKladMap}";

	}
}