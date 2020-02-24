
namespace Model.TelegramBot
{
	public abstract class LinkStruct
	{
		public string OriginalTag { get; }
		public string Url { get; }
		public string Name { get; }
		public TypeUrl TypeUrl { get; }

		protected LinkStruct(string url, string name, string originalTag, TypeUrl typeUrl)
		{
			OriginalTag = originalTag;
			Url = url;
			Name = name;
			TypeUrl = typeUrl;
		}

		public string ToHref() => $"<a href=\"{Url}\">[{Name}]</a>";
	}

	public class ImgLinkStruct : LinkStruct
	{
		public ImgLinkStruct(string url, string name, string originalTag) : base(url, name, originalTag, TypeUrl.Img)
		{

		}
	}

	public class SoundLinkStruct : LinkStruct
	{
		public SoundLinkStruct(string url, string name, string originalTag) : base(url, name, originalTag, TypeUrl.Sound)
		{

		}
	}

	public class AHrefLinkStruct : LinkStruct
	{
		public AHrefLinkStruct(string url, string name, string originalTag) : base(url, name, originalTag, TypeUrl.AHref)
		{

		}
	}

	public enum TypeUrl
	{
		Img, Sound, AHref
	}
}

