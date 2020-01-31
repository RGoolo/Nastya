using Model.BotTypes;
using Model.BotTypes.Class.Reflection;
using Model.BotTypes.Interfaces.Messages;

namespace Nastya.Mappers
{
	public abstract class BaseMapper
	{
		protected bool CheckUsage(MapperMemberInfo info, IBotMessage msg)
		{
			if (!info.СheckUsage(msg))
				return false;

			return true;
			//ToDo check pay;
			//if (_payManager.CheckPurchased(info.InstanceAttribute, msg.User) && _payManager.CheckPurchased(info, msg.User))
			//	return true;

			throw new MessageException(msg, "А это платно(");
		}

	}
}