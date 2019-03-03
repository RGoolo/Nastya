using Model.Types.Interfaces;
using System;

namespace Model.Types.Class
{
	public class PayManager
	{
		public Guid Chat { get; }

		public PayManager(Guid chat)
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
