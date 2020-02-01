using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Speech.V1P1Beta1;
using Model.Files.FileTokens;

namespace Model.Logic.Google
{
	public class Voice
	{
		public static async Task<string> GetText(IChatFile file)
		{

			StringBuilder sb = new StringBuilder();
			var speech = SpeechClient.Create();

			RecognitionAudio audio;

			if (!file.IsLocal())
			{
				/*	WebClient wc = new WebClient(); wc.DownloadData(file.Location)
					using MemoryStream stream = new MemoryStream(wc.DownloadData(file.Location));
					using var mp3 = new Mp3FileReader(stream);
					using WaveStream pcm = WaveFormatConversionStream.CreatePcmStream(mp3);

					//var  readerStream = new Reader
					var writeStream = new StreamWriter("C");
	*/

				audio = RecognitionAudio.FetchFromUri(file.Location);
				//WaveFileWriter.CreateWaveFile(@"C:\folder\stackoverflowlogo.wav", pcm);
			}
			else
			{
				await using var stream = file.ReadStream();
				audio = RecognitionAudio.FromStream(stream);
			}
			try
			{
				
				var response = await speech.RecognizeAsync(new RecognitionConfig()
			{
				Encoding = RecognitionConfig.Types.AudioEncoding.Mp3, //ToDo From file
				 //SampleRateHertz = 16000,
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
					Encoding = RecognitionConfig.Types.AudioEncoding.Mp3,
					// SampleRateHertz = 16000,
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

