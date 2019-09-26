using System;
using System.Collections.Generic;
using System.Text;

namespace Web.DZR
{
	public partial class DzrPage
	{
		public string GetAnswerText(string code)
		{
			switch (AnswerType)
			{
				case AnswerType.NotCorrect:
					return $"❌{code}: не принят";
				case AnswerType.Correct:
					return $"✅{code}: принят";
				case AnswerType.Repeated:
					return $"🔄{code}: отправлен повторно";
				default:
					return $"⚠️{code}: не стандартный ответ: {SysMessage}";
			}
		}
	}
}
