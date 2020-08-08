using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using Model.Files.FileTokens;

namespace Model.Logic.Google
{
	public class Sheets
	{
		private readonly string _sheetsUrl;

		private static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
		private const string ApplicationName = "Nastya SheetsService";
		private readonly SheetsService _service;

		public Sheets(IChatFile fileCred, IChatFileToken token, string sheetsUrl)
		{
			_sheetsUrl = sheetsUrl;

			UserCredential credential;
			var dir = Directory.GetParent(token.FullName).FullName;
			using (var stream = fileCred.ReadStream())
			{
				var clientSecret = GoogleClientSecrets.Load(stream).Secrets;
				credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
					clientSecret,
					Scopes,
					"user",
					CancellationToken.None,
					new FileDataStore(dir, true)).Result;
			}

			_service = new SheetsService(new BaseClientService.Initializer()
			{
				HttpClientInitializer = credential,
				ApplicationName = ApplicationName,
			});
        }

		//ToDo: acync
		public void CreatePage3(string name)
		{
			var myNewSheet = new Spreadsheet();
			myNewSheet.Properties = new SpreadsheetProperties();
			myNewSheet.Properties.Title = name;
			var newSheet = _service.Spreadsheets.Create(myNewSheet).Execute();
		}

		public Task CreatePageAsync(string name)
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

			var batchUpdateRequest = _service.Spreadsheets.BatchUpdate(batchUpdateSpreadsheetRequest, _sheetsUrl);

			 return batchUpdateRequest.ExecuteAsync();
		}

		public async Task UpdateValueAsync(string pageName, string cell, string value)
		{
			String range = $"{pageName}!{cell}";  // single cell D5
			String myNewCellValue = value;
			ValueRange valueRange = new ValueRange();
			valueRange.MajorDimension = "COLUMNS";//"ROWS";//COLUMNS

			var oblist = new List<object>() { value };
			valueRange.Values = new List<IList<object>> { oblist };

			SpreadsheetsResource.ValuesResource.UpdateRequest update = _service.Spreadsheets.Values.Update(valueRange, _sheetsUrl, range);
			update.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
			
			UpdateValuesResponse result2 = await update.ExecuteAsync(new CancellationToken());
		}

		public async Task GetDataAsync()
		{
			//ToDo delete:
			//await new Task(() => Thread.Sleep(0));
			// Define request parameters.
			var a = Directory.GetCurrentDirectory();
;			var range = "A2:E";
			SpreadsheetsResource.ValuesResource.GetRequest request = _service.Spreadsheets.Values.Get(_sheetsUrl, range);

			// Prints the names and majors of students in a sample spreadsheet:
			// https://docs.google.com/spreadsheets/d/1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms/edit
			
			ValueRange response = request.Execute();
			IList<IList<Object>> values = response.Values;
			
			Console.Read();
		}
	}
}