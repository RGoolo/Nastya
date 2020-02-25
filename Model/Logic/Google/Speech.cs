using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Speech.V1P1Beta1;
using Grpc.Auth;
using Model.Files.FileTokens;

namespace Model.Logic.Google
{
	public class Voice
	{

		private static SpeechClient CreateClient(IChatFile creadFile)
		{
			var credential = GoogleCredential.FromFile(creadFile.Location).CreateScoped(SpeechClient.DefaultScopes);
			var channel = new Grpc.Core.Channel(SpeechClient.DefaultEndpoint.ToString(), credential.ToChannelCredentials());
			// Instantiates a client
			return SpeechClient.Create(channel);
		}

		private static RecognitionConfig CreateConfig(IChatFileToken file) => new RecognitionConfig()
			{
				Encoding = file.FileName.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase)
					? RecognitionConfig.Types.AudioEncoding.Mp3
					: RecognitionConfig.Types.AudioEncoding.OggOpus,
				SampleRateHertz = 16000,
				LanguageCode = "ru",
			};

		public static async Task<string> GetText(IChatFile file, IChatFile creadFile)
		{
			StringBuilder sb = new StringBuilder();
			var speech = CreateClient(creadFile);

			RecognitionAudio audio;

			if (!file.IsLocal())
			{
				audio = RecognitionAudio.FetchFromUri(file.Location);
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

