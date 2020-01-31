using System.Security;

namespace Model.BotTypes.Interfaces
{
	interface ISecurity
	{
		void SetPassword(SecureString securityString, string key);
		SecureString GetPassword(string key);
	}
}