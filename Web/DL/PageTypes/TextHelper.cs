namespace Web.DL
{
	public static class TextHelper
	{
		public static string ToText(this TypeCode code, string codeText) => code switch
		{
			TypeCode.Received => $"✅{codeText}: принят",
			TypeCode.NotReceived => $"❌{codeText}: не принят",
			_ => $"⚠️{codeText} Ошибка, попробуйте еще раз",
		};
	}
}