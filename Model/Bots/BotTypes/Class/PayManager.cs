using Model.Bots.BotTypes.Interfaces;
using Model.Bots.BotTypes.Interfaces.Ids;
using Model.Bots.BotTypes.Interfaces.Messages;

namespace Model.Bots.BotTypes.Class
{
	public class PayManager
	{
		public IChatId Chat { get; }

		public PayManager(IChatId chat)
		{
			Chat = chat;
		}

		public bool CheckPurchased(IPay pay, IUser user)
		{
			if (user.Type == Enums.TypeUser.Developer)
				return true;

			return !pay.Paid;
		}
	}
}
