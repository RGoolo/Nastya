using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Bson;

namespace Model.Logic.Google
{
	public class WorkerSheets
	{
		private Sheets _sheets = new Sheets();
		public string SheetToken { get; set; }

		public WorkerSheets()
		{

		}

		//ToDo acync
		public void CreateSheetsAsync(string pageName)
		{
			try
			{
				_sheets.CreatePage(SheetToken, pageName);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
		}

		public void UpdateDlPage(string pageName, string cell, string value)
		{
			_sheets.UpdateValue(SheetToken, pageName, cell, value);
		}
	}
}