using Model.Types.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;

namespace Model.Types.Class
{
	public static class SecurityEnvironment
	{
		private const string prefix = "bot_nastya_";

		public static SecureString GetPassword(string key)
		{
			var value = Environment.GetEnvironmentVariable(prefix + key, EnvironmentVariableTarget.User);
			if (value == null) return null;

			var ss = new SecureString();
			foreach (var c in value)
				ss.AppendChar(c);
			return ss;
		}

		public static void SetPassword(SecureString securityString, string key) =>
			Environment.SetEnvironmentVariable(prefix + key, new NetworkCredential(string.Empty, securityString).Password, EnvironmentVariableTarget.User);
		
	}
}
