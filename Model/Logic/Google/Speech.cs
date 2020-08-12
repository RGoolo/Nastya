using System.Text;
using System.Threading.Tasks;
using BotModel.Files.FileTokens;
using Google.Cloud.Speech.V1P1Beta1;

namespace Model.Logic.Google
{
	public class Voice
	{
        private static SpeechClient CreateClient(IChatFile credFile)
		{
            var builder = new SpeechClientBuilder()
			{
                CredentialsPath = credFile.FullName,
            };
			return builder.Build();
		}

		private static RecognitionConfig CreateConfig(IChatFileToken file) => new RecognitionConfig()
			{
				Encoding = RecognitionConfig.Types.AudioEncoding.Mp3,
				SampleRateHertz = 16000,
				LanguageCode = "ru",
			};

		public static async Task<string> GetText(IChatFile file, IChatFile credFile)
		{
			StringBuilder sb = new StringBuilder();
			var speech = CreateClient(credFile);

			RecognitionAudio audio;

			if (!file.IsLocal())
			{
				audio = RecognitionAudio.FetchFromUri(file.FullName);
			}
			else
			{
				await using var stream = file.ReadStream();
				audio = RecognitionAudio.FromStream(stream);
			}

			try
			{
				var response = await speech.RecognizeAsync(CreateConfig(file), audio);

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
				var response2 = await speech.LongRunningRecognizeAsync(CreateConfig(file), audio);

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

