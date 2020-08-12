using BotModel.HttpMessages.Simple;
using Web.DL;

namespace Web.HttpMessages
{
	public static class HttpMessagesExtensions
	{

		public static IBasicHttpMessages AddDlThrowMode(this IBasicHttpMessages messages)
		{
			return new DlThrowAuthorizationMessages(messages);
		}

	}

}