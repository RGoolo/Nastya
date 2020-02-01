using System;
using System.Threading.Tasks;
using Model.Files.FileTokens;
using Model.Logic.Model;
using ILogger = Model.Logger.ILogger;

namespace Model.Logic.Google
{
	public class WorkerSheets
	{
		private readonly string _sheetsUrl;
		private Sheets _sheets;// ToDo token = new Sheets();

		public ILogger Log = Logger.Logger.CreateLogger(nameof(WorkerSheets));

		public WorkerSheets(IChatFile fileCred, IChatFile fileToken, string sheetsUrl)
		{
			_sheetsUrl = sheetsUrl;
			_sheets ??= new Sheets(fileCred, fileToken, sheetsUrl);
		}

		public async Task<string> CreateSheetsAsync(string pageName)
		{
			try
			{
				await _sheets.CreatePageAsync( pageName);
				return $"Страница {pageName} создана";
			}
			catch (Exception e)
			{
				Log.Error(e);
				throw new GameException(e);
			}
		}

		public Task UpdateDlPage(string pageName, string cell, string value)
		{
			return _sheets.UpdateValueAsync( pageName, cell, value);
		}
	}
}