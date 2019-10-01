using System;
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


			try
			{
				var response = await speech.RecognizeAsync(new RecognitionConfig()
				{
					Encoding = RecognitionConfig.Types.AudioEncoding.OggOpus,
					SampleRateHertz = 16000,
					LanguageCode = "ru",

				}, audio);


				foreach (var result in response.Results)
				{
					foreach (var alternative in result.Alternatives)
					{
						sb.Append(alternative.Transcript + "\n");
					}
				}

				return sb.ToString();

			}
			catch

			{


				var response2 = await speech.LongRunningRecognizeAsync(new RecognitionConfig
				{
					Encoding = RecognitionConfig.Types.AudioEncoding.OggOpus,
					SampleRateHertz = 16000,
					LanguageCode = "ru",
					

				}, audio);

				while (!response2.IsCompleted)
				{
					if (response2.Result != null && response2.Metadata != null)
					{
						LongRunningRecognizeMetadata meta = response2.Metadata;
						var ProgressValue = meta.ProgressPercent;

						//Add results to variable "TranslatedText" here -how?
					}
					
				}

				var a = response2.PollUntilCompletedAsync();
				return null;
			}
		}
	}
}

