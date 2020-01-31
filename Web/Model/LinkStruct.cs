using static Web.Base.WebHelper;

namespace Web.Base
{
	public abstract class LinkStruct
	{
		public string Url;
		public string Name;
		public TypeUrl TypeUrl;

		protected LinkStruct(string url, string name, TypeUrl typeUrl)
		{
			Url = url;
			Name = name;
			TypeUrl = typeUrl;
		}
	}

	public class ImgLinkStruct : LinkStruct
	{
		public ImgLinkStruct(string url, string name) : base(url, name, TypeUrl.Img)
		{

		}
	}

	public class SoundLinkStruct : LinkStruct
	{
		public SoundLinkStruct(string url, string name) : base(url, name, TypeUrl.Sound)
		{

		}
	}

	public class AHrefLinkStruct : LinkStruct
	{
		public AHrefLinkStruct(string url, string name) : base(url, name, TypeUrl.AHref)
		{

		}
	}
}

