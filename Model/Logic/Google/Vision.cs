using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotModel.Files.FileTokens;
using Google.Cloud.Vision.V1;

namespace Model.Logic.Google
{
	public class Vision
	{
		public enum TypeImgFunc
		{
			Text, Properties, label, CropHint, LandMarks
		}

		private static string Link(string url, string name) => $"<a href=\"{url}\">{name}</a>";

		private static Image GetImg(IChatFile file) => file.FileType.IsLocal() ? Image.FromStream(file.ReadStream()) : Image.FromUri(file.FullName);

		private static ImageAnnotatorClient CreateClient(IChatFileFactory factory)
        {
            var credFile = factory.SystemFile(SystemChatFile.RecognizeCredentials);

			var builder = new ImageAnnotatorClientBuilder
            {
                 CredentialsPath = credFile.FullName,
            };
            return builder.Build();
        }

		public static async Task<string> GetTextAsync(IChatFile fileTo, IChatFileFactory factory)
		{
			var image = GetImg(fileTo);
			var client = CreateClient(factory);
			StringBuilder sb = new StringBuilder();
			var responce = await client.DetectTextAsync(image);
			if (responce == null)
				return string.Empty;

			foreach (var a in responce)
				sb.Append(a.Description);

			return sb.ToString();
		}

		public static async Task<string> GetWebAsync(IChatFile file, IChatFileFactory factory)
		{
			StringBuilder sb = new StringBuilder();
			var image = GetImg(file);

			var client = CreateClient(factory);
			var annotation = await client.DetectWebInformationAsync(image);
			if (annotation == null)
				return string.Empty;

			if (annotation.FullMatchingImages.Any()) sb.Append("MatchingImage:");
			foreach (var url in annotation.FullMatchingImages)
				sb.Append(Link(url.Url, $" mi score {url.Score};"));

			if (annotation.PagesWithMatchingImages.Any()) sb.Append("PagesWithMatchingImages:");
			foreach (var url in annotation.PagesWithMatchingImages)
				sb.Append(Link(url.Url, $" page score {url.Score};"));

			if (annotation.PartialMatchingImages.Any()) sb.Append("PartialMatchingImages:");
			foreach (var url in annotation.PartialMatchingImages)
				sb.Append(Link(url.Url, $" partial score {url.Score};"));

			if (annotation.WebEntities.Any()) sb.Append("WebEntity:\n");
			foreach (var url in annotation.WebEntities)
				sb.Append($"Score:{url.Score}\tDescription:\t{url.Description};\n");

			return sb.ToString();
		}

		public static async Task<string> GetLogoAsync(IChatFile file, IChatFileFactory factory)
		{
			StringBuilder sb = new StringBuilder();

			var image = GetImg(file);

			var client = CreateClient(factory);

			var responce = await client.DetectLogosAsync(image);
			if (responce != null)
				foreach (var a in responce)
					sb.Append(a.Description);

			return sb.ToString();
		}

		public static async Task<string> GetLandmarkAsync(IChatFile file, IChatFileFactory factory)
		{
			StringBuilder sb = new StringBuilder();
			var image = GetImg(file);

			var client = CreateClient(factory);

			var responce = await client.DetectLandmarksAsync(image);
			if (responce != null)
				foreach (var a in responce)
					sb.Append(a.Description);

			return sb.ToString();
		}

		public static async Task<string> GetDocAsync(IChatFile file, IChatFileFactory factory)
		{
			StringBuilder sb = new StringBuilder();
			var image = GetImg(file);

			var client = CreateClient(factory);
			var response = await client.DetectDocumentTextAsync(image);
			if (response != null)
				foreach (var page in response.Pages)
					foreach (var block in page.Blocks)
						foreach (var paragraph in block.Paragraphs)
							sb.Append(string.Join("\n", paragraph.Words));

			return sb.ToString();
		}
	}
}
