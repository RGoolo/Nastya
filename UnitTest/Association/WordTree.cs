using WordService.TreeNode;
using Xunit;

namespace UnitTest.Association
{
	public class WordTree
	{
		[Fact] public void CheckAssociation()
		{
			var wt = new WordsTree();
			wt.AddWord("мир");
			wt.AddWord("труд");
			wt.AddWord("май");

			wt.AddAssociation("мир", "труд");
			wt.AddAssociation("труд", "май");
			wt.AddAssociation("май", "мир");

			var list  = wt.GetAssociation("мир");

			Assert.True(list.Count == 2);
		}
		
	}
}