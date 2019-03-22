using System;
using System.Net;
using System.Security;

namespace Model.Types.Class
{
	public static class SecurityEnvironment
	{
		private const string Prefix = "bot_nastya_";

		public static SecureString GetPassword(string key)
		{
			var value = Environment.GetEnvironmentVariable(Prefix + key, EnvironmentVariableTarget.User);
			if (value == null) return null;

			var ss = new SecureString();
			foreach (var c in value)
				ss.AppendChar(c);
			return ss;
		}

		public static void SetPassword(SecureString securityString, string key) =>
			Environment.SetEnvironmentVariable(Prefix + key, new NetworkCredential(string.Empty, securityString).Password, EnvironmentVariableTarget.User);
		
	}
}
