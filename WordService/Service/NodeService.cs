using WordService.TreeNode;

namespace WordService.Service
{
	public class NodeService
	{
		public static void AddAssociation(Node first, Node second)
		{
			first.AddAssociation(second);
			second.AddAssociation(first);
		}
	}
}