using System;

namespace Web.Base
{
	public class AuthorizationFailedException : Exception
	{
		public AuthorizationFailedException() : base("Не удалось подключиться. Проверте логин/пароль")
		{

		}
	}
}