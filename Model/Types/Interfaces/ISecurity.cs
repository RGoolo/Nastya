using System;
using System.Collections.Generic;
using System.Security;
using System.Text;

namespace Model.Types.Interfaces
{
	interface ISecurity
	{
		void SetPassword(SecureString securityString, string key);
		SecureString GetPassword(string key);
	}
}