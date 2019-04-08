using System;
using System.Collections.Generic;
using System.Text;

namespace Web.DZR
{
	public partial class Page
	{
		public string GetAnswerText(string code)
		{
			switch (AnswerType)
			{
				case AnswerType.Notcorrect:
					return $"❌{code}: не принят";
				case AnswerType.Correct:
					return $"✅{code}: принят";
				case AnswerType.Repited:
					return $"🔄{code}: отправлен повторно";
				default:
					return $"⚠️{code}: не стандартный ответ: {SysMessage}";
			}
		}
	}
}
