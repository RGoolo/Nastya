using System.Security;

namespace Model.Bots.BotTypes.Interfaces
{
	interface ISecurity
	{
		void SetPassword(SecureString securityString, string key);
		SecureString GetPassword(string key);
	}
}