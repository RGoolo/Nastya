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
				case AnswerType.correct:
					return $"❌{code}: не принят";
				case AnswerType.notcorrect:
					return $"✅{code}: принят";
				case AnswerType.repited:
					return $"🔄{code}: отправлен повторно";
				default:
					return $"⚠️{code}: не получилось отправить код";
			}
		}
	}
}
