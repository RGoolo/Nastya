using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Speech.V1;
using Model.Types.Interfaces;

namespace Model.Logic.Google
{
	public class Voice
	{

		public static async Task<string> GetText(IChatFileWorker worker, IFileToken file)
		{
			//ToDo  async

			StringBuilder sb = new StringBuilder();
			var speech = SpeechClient.Create();

			RecognitionAudio audio;

			using (var stream = worker.ReadStream(file))
				audio = RecognitionAudio.FromStream(stream);

			var response = await speech.RecognizeAsync(new RecognitionConfig()
			{
				Encoding = RecognitionConfig.Types.AudioEncoding.OggOpus,
				SampleRateHertz = 16000,
				LanguageCode = "ru",
			}, audio);

			var res = response.Results;
			foreach (var result in res)
			{
				foreach (var alternative in result.Alternatives)
				{
					sb.Append(alternative.Transcript + "\n");
				}
			}
			return sb.ToString();
		}
	}
}

