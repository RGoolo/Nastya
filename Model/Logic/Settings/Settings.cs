﻿using System;
using System.Collections.Generic;

namespace Model.Logic.Settings
{
	[Flags, Serializable]
	public enum TypeGame
	{
		Unknown = 0x0,
		Dzzzr = 0x1,
		DeadLine = 0x2,
		Dummy = 0x4,
		Prequel = 0x8,
		Lite = 0x10,

		DzzzrLite = Lite | Dzzzr,
		DzzzrPrequel = Dzzzr| Prequel,
		DzzzrLitePrequel = Dzzzr | Prequel | Lite,
	}

	[Serializable]
	public class KeyVal
	{
		public string Key { get; set; }
		public string Val { get; set; }
		public KeyVal() { }
		public KeyVal(string key, string val)
		{
			Key = key;
			Val = val;
		}
	}

	[Serializable]
	public class Settings
	{
		[NonSerialized]
		private readonly object _lock = new object();

		[NonSerialized]
		Dictionary<string, string> _properties = new Dictionary<string, string>();

		public List<KeyVal> Setting { get; set; }

		public void Clear()
		{
			lock (_lock)
			{
				_properties.Clear();
				Setting.Clear();
			}
		}

		public void SetDictionary()
		{
			lock (_lock)
			{
				_properties.Clear();

				Setting.ForEach( x => _properties.Add(x.Key, x.Val));
			}
		}
		public void SetList()
		{
			lock (_lock)
			{
				Setting.Clear();
				foreach (var keyVal in _properties)
				{
					Setting.Add(new KeyVal(keyVal.Key, keyVal.Value));
				}
			}
		}
		
		public Guid ChatGuid { get; set; }
		public TypeGame TypeGame { get; set; }

		public void SetValue(string name, string value)
		{
			lock (_lock)
			{
				if (_properties.ContainsKey(name))
					_properties.Remove(name);

				_properties.Add(name, value);
			}
		}

		public Settings(Guid chatId)
		{
			ChatGuid = chatId;
			Setting = new List<KeyVal>();
		}

		public Settings() { }

		public virtual string GetValue(string name, string @default = default(string))
		{
			lock (_lock)
			{
				return _properties.GetValueOrDefault(name, @default);
			}
		}

		public virtual bool GetValueBool(string name, bool @default = default(bool))
		{
			lock (_lock)
			{
				if (!_properties.ContainsKey(name)) return @default;
				return bool.TryParse(_properties.GetValueOrDefault(name), out var b) ? b : @default;
			}
		}

		public virtual long GetValueLong(string name, long @default = default(long))
		{
			lock (_lock)
			{
				if (!_properties.ContainsKey(name)) return @default;
				return long.TryParse(_properties.GetValueOrDefault(name), out var b) ? b : @default;
			}
		}
	}
}
