using System.Threading.Tasks;
using BotModel.Bots.BotTypes;
using BotModel.Files;
using BotModel.HttpMessages;
using Model.Settings;
using Web.DL.PageTypes;
using HttpMessagesFactory = Web.HttpMessages.HttpMessagesFactory;

namespace Web.DL
{
	public interface IDlValidator
	{
		Task<DLPage> SendCode(string code, DLPage page);
		Task<DLPage> LogIn();
		Task<DLPage> GetNextPage();
	}

	public static class FactoryValidator
	{
		public static IDlValidator CreateValidator(IChatService settings) => 
			settings.TypeGame.IsDummy() 
				? (IDlValidator) new DlLocalValidator(settings)
				: (IDlValidator) new DlWebValidator(settings);


		private class DlLocalValidator : IDlValidator
		{
			private readonly IChatService _settings;

			public DlLocalValidator(IChatService settings)
			{
				_settings = settings;
			}

			public Task<DLPage> SendCode(string code, DLPage page) => GetNextPage();
			public Task<DLPage> LogIn() => GetNextPage();
			public async Task<DLPage> GetNextPage() => GetNextPage(await GetPage());
			private DLPage GetNextPage(string text) => PageConstructor.CreateNewPage(text);
			private Task<string> GetPage() => FileHelper.ReadToEndAsync(_settings.Web.Domen);
		}

		private class DlWebValidator : IDlValidator
		{
			private readonly IChatService _settings;
			private readonly IHttpMessages _httpMessages;

			public DlWebValidator(IChatService settings)
			{
				_settings = settings;
				_httpMessages = HttpMessagesFactory.DeadlineThrowAuthorizationMessages();
			}

			private string Domain => _settings.Web.Domen;
			private string Site => $@"http://{Domain}/";
			private string Login => _settings.Game.Login;
			private string Password => _settings.Game.Password;

			private string LoginContext =>
				$@"socialAssign=0&Login={Login}&Password={Password}&EnButton1=Sign+In&ddlNetwork=1";

			private string CreateUrl()
			{
				var result = $@"{Site}{_settings.Web.BodyRequest}/{_settings.Web.GameNumber}/";

				if (!_settings.DlGame.Sturm) return result;

				var lvl = _settings.Game.Level;
				if (!string.IsNullOrEmpty(lvl))
					return result + "?level=" + lvl;
				return result;
			}

			private string LoginUrl() => $@"{Site}Login.aspx";

			private static string GetContextSetCode(string code, DLPage page) =>
				$"LevelId={page?.LevelId}&LevelNumber={page?.LevelNumber}&LevelAction.Answer=" + code;

			public async Task<DLPage> SendCode(string code, DLPage page) =>
				GetNextPage(await _httpMessages.GetText(CreateUrl(), GetContextSetCode(code, page)));

			public async Task<DLPage> LogIn()
			{
				try
				{
					await _httpMessages.Response(CreateUrl());
				}
				catch
				{

				}

				try
				{
					await _httpMessages.GetText(LoginUrl(), LoginContext);
				}
				catch
				{

				}

				var text = await _httpMessages.GetText(CreateUrl());
				return GetNextPage(text);
			}

			public async Task<DLPage> GetNextPage() => GetNextPage(await GetPage());
			private DLPage GetNextPage(string text) => PageConstructor.CreateNewPage(text);
			private Task<string> GetPage() => _httpMessages.GetText(CreateUrl());
		}
	}
}
