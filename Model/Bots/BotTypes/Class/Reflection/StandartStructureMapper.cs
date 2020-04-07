using System;
using System.Collections.Generic;

namespace Model.Bots.BotTypes.Class.Reflection
{
	public static class StandardStructureMapper
	{
		private static readonly Dictionary<Type, Func<string, object>> GetValues = new Dictionary<Type, Func<string, object>>()
		{
			{ typeof(Guid?), (value) => GetGuid(value)},
			{ typeof(Guid), (value) => new Guid(value)},
			{ typeof(int?), (value) => GetInt(value)},
			{ typeof(int), (value) => int.Parse(value)},
			{ typeof(long?), (value) => GetLong(value)},
			{ typeof(long), (value) => long.Parse(value)},
			{ typeof(DateTime?), (value) => GetDateTime(value)},
			{ typeof(DateTime), (value) => DateTime.Parse(value)},
			{ typeof(bool?), (value) => GetBool(value)},
			{ typeof(bool), (value) => bool.Parse(value)},
			{ typeof(string), (value) => value},
		};

		public static object GetType(Type type, string str)
		{
			return type.IsEnum? Enum.Parse(type, str) : GetValues[type](str);
		}

		public static object GetDefault(Type type) => type.IsValueType ? Activator.CreateInstance(type) : null;

		static Guid? GetGuid(string value) => string.IsNullOrEmpty(value) ? (Guid?)null : new Guid(value);
		static int? GetInt(string value) => string.IsNullOrEmpty(value) ? (int?)null : int.Parse(value);
		static long? GetLong(string value) => string.IsNullOrEmpty(value) ? (long?)null : long.Parse(value);
		static DateTime? GetDateTime(string value) => string.IsNullOrEmpty(value) ? (DateTime?)null : DateTime.Parse(value);
		static bool? GetBool(string value) => string.IsNullOrEmpty(value) ? (bool?)null : bool.Parse(value);
	}
}