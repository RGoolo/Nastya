using System.Collections.Generic;
using System.Linq;
using System.Text;
using WordService.Service;

namespace WordService.TreeNode
{
	public class WordsTree
	{
		private readonly Node _node = Node.CreateRootNode();

		public WordsTree()
		{

		}

		public List<string> GetAssociation(string str)
		{
			var node = _node.Get(new StringBuilder(str));
			return node?.Association?.Select(n => n.GetWord().ToString()).ToList();
		}

		public Node AddWord(string str)
		{
			return _node.AddWord(new StringBuilder(str));
		}

		public void AddAssociation(string first, string second)
		{
			AddWord(first);
			var firstNode = _node.Get(new StringBuilder(first));
			AddWord(second);
			var secondNode = _node.Get(new StringBuilder(second));

			NodeService.AddAssociation(firstNode, secondNode);
		}
	}
}