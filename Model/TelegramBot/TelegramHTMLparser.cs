using System.Text.RegularExpressions;

namespace Model.TelegramBot
{
    public class TelegramHTML
    {
		public string RemoveAllTag(string str)
		{
			return Regex.Replace(str, "<[^>]+>", string.Empty);
		}

		public string RemoteTag(string str)
		{
			/*var result = Regex.Replace(str, "<br>|<p>","\n");
			// ToDo
			// Лишняя операция, но не составить Regex что бы не удалял <\a> 
			result = result.Replace()*/
			var aaa =  Regex.Replace(Regex.Replace(str.Replace("<br>", "\n").Replace("<p>", "\n").Replace("</a>", "<a>"), "<[^((i)|(a))][^>]*>", string.Empty), "\n{2,}", "\n");
			//return Regex.Replace(aaa, "&", "&amp;");
			return Regex.Replace(aaa, "<a>", "</a>");

			//return null;
		}
	}
}
