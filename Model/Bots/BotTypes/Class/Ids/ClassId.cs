using System;

namespace Model.Bots.BotTypes.Class.Ids
{
	public abstract class ClassId<T>
	{
		protected ClassId(T value)
		{
			Get = value;
		}

		public T Get { get; }

		public abstract Guid GetId { get; }

		public override string ToString() => GetId.ToString();

		
		public override int GetHashCode() => Get.GetHashCode();

		public override bool Equals(object obj) => obj is ClassId<T> guid && Get.Equals(guid.Get);
	}
}