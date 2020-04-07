using System;
using System.Collections.Generic;
using Model.Bots.BotTypes.Class.Ids;
using Model.Bots.BotTypes.Enums;
using Model.Bots.BotTypes.Interfaces.Ids;

namespace Model.Logic.Settings
{


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

		[NonSerialized] readonly Dictionary<string, string> _properties = new Dictionary<string, string>();

		public List<KeyVal> Setting { get; set; }

		//[NonSerialized] public List<Answer> Answers = new List<Answer> ();

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
				if (_properties.ContainsKey(name.ToLower()))
					_properties.Remove(name.ToLower());

				_properties.Add(name.ToLower(), value);
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

		public virtual Guid GetValueGuid(string name, Guid @default = default(Guid))
		{
			lock (_lock)
			{
				if (!_properties.ContainsKey(name)) return @default;
				return Guid.TryParse(_properties.GetValueOrDefault(name), out var b) ? b : @default;
			}
		}

		public virtual IMessageId GetMessageId(string name, IMessageId @default = default)
		{
			lock (_lock)
			{
				if (!_properties.ContainsKey(name)) return @default;

				var guid = Guid.TryParse(_properties.GetValueOrDefault(name), out var b) ? b :Guid.Empty;
				return new MessageGuid(guid, new ChatGuid(ChatGuid));
			}
		}
	}
}
