using System.Security;

namespace Model.Types.Interfaces
{
	interface ISecurity
	{
		void SetPassword(SecureString securityString, string key);
		SecureString GetPassword(string key);
	}
}