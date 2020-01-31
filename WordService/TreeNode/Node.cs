using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using System.Text;

namespace WordService.TreeNode
{
	public class Node //ToDo to struct
	{
		public char Value { get; }

		private Dictionary<char, Node> _children; // = new Dictionary<char, Node>()

		public Node Parent { get; }
		public bool IsFinish { get; set; }

		public List<Node> Association { get; private set; }

		public void AddAssociation(Node node)
		{
			if (Association == null) Association = new List<Node>();
			Association.Add(node);
		}

		public static Node CreateRootNode()
		{
			return new Node();
		}

		private Node()
		{

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="endWord">Will change</param>
		/// <param name="parent"></param>
		public Node(StringBuilder endWord, Node parent)
		{
			Parent = parent;
			Value = endWord[0];

			AddWord(endWord.Remove(0, 1));
		}

		// public override int GetHashCode() => Value.GetHashCode();
		// public override bool Equals(object value) => value is Node node && node?.Value == Value;

		public Node AddWord(StringBuilder value)
		{
			if (value.Length != 0) return GetOrCreate(value);
			
			IsFinish = true;
			return this;
		}

		public Node Get(StringBuilder value)
		{
			if (value.Length == 0) return this;

			return !_children.TryGetValue(value[0], out var existingNode) ? null : existingNode.Get(value.Remove(0, 1));
		}

		private Node GetOrCreate(StringBuilder value)
		{
			if (_children == null)
				_children = new Dictionary<char, Node>();

			var cr = value[0];

			if (_children.TryGetValue(cr, out var existingNode))
			{
				existingNode.AddWord(value.Remove(0, 1));
				return existingNode;
			}

			var newNode = new Node(value, this);
			_children.Add(cr, newNode);
			return newNode;
		}
	}

	public static class NodeExtension
	{
		public static StringBuilder GetWord(this Node node)
		{
			var sb = node.Value != '\0' ? node.Parent.GetWord() : new StringBuilder();
			sb.Append(node.Value.ToString());
			return sb;
		}

		/*
		public static List<Node> GetMaskWord(this Node node,List<char> list)
		{
			foreach (var c in list)
			{
				
			}
		}*/
	}
}