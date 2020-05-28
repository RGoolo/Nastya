using System;
using System.Net;
using System.Security;

namespace Model.Bots.BotTypes.Class
{
	public static class SecurityEnvironment
	{
		private const string Prefix = "bot_nastya_";

		public static SecureString GetPassword(params string[] keys)
		{
			var value = Environment.GetEnvironmentVariable(Prefix + string.Join("_", keys), EnvironmentVariableTarget.User);
			if (value == null) return null;

			var ss = new SecureString();
			foreach (var c in value)
				ss.AppendChar(c);
			return ss;
		}

        public static void SetPassword(SecureString securityString, params string[] keys)
        {
            Environment.SetEnvironmentVariable(Prefix + string.Join("_", keys), new NetworkCredential(string.Empty, securityString).Password, EnvironmentVariableTarget.User);
		}
        
		public static void SetPassword(string securityString, params string[] keys) => Environment.SetEnvironmentVariable(Prefix + string.Join("_", keys), securityString, EnvironmentVariableTarget.User);
        public static string GetTextPassword(params string[] keys) => Environment.GetEnvironmentVariable(Prefix + string.Join("_", keys), EnvironmentVariableTarget.User);
    }
}
