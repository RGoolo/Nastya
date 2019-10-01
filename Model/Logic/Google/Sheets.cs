using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;

namespace Model.Logic.Google
{
	public class Sheets
	{
		// If modifying these scopes, delete your previously saved credentials
		// at ~/.credentials/sheets.googleapis.com-dotnet-quickstart.json
		private static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
		private const string ApplicationName = "Nastya SheetsService";

		private readonly SheetsService _service;

		public Sheets()
		{
			UserCredential credential;

			using (var stream =
				new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
			{
				// The file token.json stores the user's access and refresh tokens, and is created
				// automatically when the authorization flow completes for the first time.
				string credPath = "token.json";
				credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
					GoogleClientSecrets.Load(stream).Secrets,
					Scopes,
					"user",
					CancellationToken.None,
					new FileDataStore(credPath, true)).Result;
				Console.WriteLine("Credential file saved to: " + credPath);
			}

			// Create Google Sheets API service.
			_service = new SheetsService(new BaseClientService.Initializer()
			{
				HttpClientInitializer = credential,
				ApplicationName = ApplicationName,
			});

		}

		//ToDo: acync
		public void CreatePage3(string spreadsheetId, string name)
		{
			var myNewSheet = new Spreadsheet();
			myNewSheet.Properties = new SpreadsheetProperties();
			myNewSheet.Properties.Title = name;
			var newSheet = _service.Spreadsheets.Create(myNewSheet).Execute();
		}

		public void CreatePage(string spreadsheetId, string name)
		{
			var addSheetRequest = new AddSheetRequest();
			addSheetRequest.Properties = new SheetProperties();
			addSheetRequest.Properties.Title = name;
			BatchUpdateSpreadsheetRequest batchUpdateSpreadsheetRequest = new BatchUpdateSpreadsheetRequest();
			batchUpdateSpreadsheetRequest.Requests = new List<Request>();
			batchUpdateSpreadsheetRequest.Requests.Add(new Request
			{
				AddSheet = addSheetRequest
			});

			var batchUpdateRequest = _service.Spreadsheets.BatchUpdate(batchUpdateSpreadsheetRequest, spreadsheetId);

			batchUpdateRequest.Execute();
		}

		public void UpdateValue(string spreadsheetId, string pageName, string cell, string value)
		{
			String range = $"{pageName}!{cell}";  // single cell D5
			String myNewCellValue = value;
			ValueRange valueRange = new ValueRange();
			valueRange.MajorDimension = "COLUMNS";//"ROWS";//COLUMNS

			var oblist = new List<object>() { value };
			valueRange.Values = new List<IList<object>> { oblist };

			SpreadsheetsResource.ValuesResource.UpdateRequest update = _service.Spreadsheets.Values.Update(valueRange, spreadsheetId, range);
			update.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
			UpdateValuesResponse result2 = update.Execute();
		}

		public async Task GetDataAsync(string spreadsheetId)
		{
			//ToDo delete:
			//await new Task(() => Thread.Sleep(0));
			// Define request parameters.
			var a = Directory.GetCurrentDirectory();
;			var range = "A2:E";
			SpreadsheetsResource.ValuesResource.GetRequest request = _service.Spreadsheets.Values.Get(spreadsheetId, range);

			// Prints the names and majors of students in a sample spreadsheet:
			// https://docs.google.com/spreadsheets/d/1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms/edit
			
			ValueRange response = request.Execute();
			IList<IList<Object>> values = response.Values;
			if (values != null && values.Count > 0)
			{
				Console.WriteLine("Name, Major");
				foreach (var row in values)
				{
					// Print columns A and E, which correspond to indices 0 and 4.
					Console.WriteLine("{0}, {1}", row[0], row[4]);
				}
			}
			else
			{
				Console.WriteLine("No data found.");
			}
			Console.Read();
		}
	}
}