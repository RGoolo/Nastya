using System;
using ILogger = Model.Logger.ILogger;

namespace Model.Logic.Google
{
	public class WorkerSheets
	{
		private Sheets _sheets;// ToDo token = new Sheets();
		public string SheetToken { get; set; }
		public ILogger Log = Logger.Logger.CreateLogger(nameof(WorkerSheets));

		public WorkerSheets()
		{

		}

		public void CreateSheetsAsync(string pageName)
		{
			try
			{
				_sheets.CreatePage(SheetToken, pageName);
			}
			catch (Exception e)
			{
				Log.Error(e);
			}
		}

		public void UpdateDlPage(string pageName, string cell, string value)
		{
			_sheets.UpdateValue(SheetToken, pageName, cell, value);
		}
	}
}