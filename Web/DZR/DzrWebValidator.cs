using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.BotTypes.Enums;
using Model.HttpMessages;
using Model.Logic.Settings;
using HttpMessagesFactory = Web.HttpMessages.HttpMessagesFactory;

namespace Web.DZR
{
	public class DzrWebValidator
	{
		private ISettings _settings { get; }
		public IHttpMessages messages;
		private string _url;
		private readonly string _tempDzrUrl =  new DzrUrl().ToString();
		public DzrWebValidator(ISettings settings)
		{
			_settings = settings;
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

			messages = HttpMessagesFactory.Dzzzr();
		}

		public async Task<DzrPage> SendCode(string code, DzrTask task) => GetNextPage(await messages.GetText(GetUrl(), GetContextSetCode(code, task)));

		public async Task<DzrPage> LogIn()
		{
			await messages.Response(LogInUrl(), LogInContext());
			return await GetNextPage();
		}

		private DzrPage GetNextPage(string html) => new DzrPage(html, GetUrl());

		public async Task<DzrPage> GetNextPage() => new DzrPage(await messages.GetText(GetUrl()), GetUrl());

		public bool IsLogOut(DzrPage page) => page == null || page.Type == PageType.NotFound;

		public string GetContextSetCode(string code, DzrTask task) => task?.GetPostForCode(code);

		public string LogInContext() => $@"notags=&action=auth&login={_settings.Game.Login}&password={_settings.Game.Password}";

		public string GetUrl() => GetBaseUrl() + "?" + _tempDzrUrl;

		private string GetBaseUrl()
		{
			if (_url != null)
				return _url;

			if ((_settings.TypeGame & TypeGame.Dummy) == TypeGame.Dummy)
				return _settings.Web.Domen.Split('\\').SkipLast(1).Aggregate((x, y) => x + "\\" + y);

			_url = $@"http://{_settings.Web.Domen}/{_settings.Web.BodyRequest}/go/";

			return _url;
		}

		public string LogInUrl() => GetBaseUrl();
	}
}