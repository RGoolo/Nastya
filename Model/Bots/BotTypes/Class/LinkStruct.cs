
namespace Model.Bots.BotTypes.Class
{
	public abstract class LinkStruct
	{
        public LocationFileType Location;
        public string OriginalTag { get; }
		public string Url { get; }
		public string Name { get; }
		public TypeUrl TypeUrl { get; }
        

		protected LinkStruct(string url, string name, string originalTag, TypeUrl typeUrl)
		{
            Location = (url != null && url.Length > 2 && url[1] == ':') ? LocationFileType.Local : LocationFileType.Internet;
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

    public enum LocationFileType
    {
		Local, Internet
    }

	public enum TypeUrl
	{
		Img, Sound, AHref
	}
}

