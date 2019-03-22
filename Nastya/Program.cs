namespace Nastya
{
	class Program
	{
		static void Main(string[] args)
		{
			StartBot();	
		}
	
		private static void StartBot()
		{
			var StartBot = new ManagerBots();
			StartBot.Wait();
		}
	}
}