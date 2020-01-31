using System.Collections.Generic;
using System.Net;
using Model.HttpMessages.Simple;
using Web.DL;

namespace Web.HttpMessages.Simple
{
	public static class HttpMessagesExtensions
	{

		public static IBasicHttpMessages AddDlThrowMode(this IBasicHttpMessages messages)
		{
			return new DlThrowAuthorizationMessages(messages);
		}

	}

}