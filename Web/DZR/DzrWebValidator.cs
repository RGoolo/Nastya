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
		private readonly ISettings _settings;
		public IHttpMessages Messages;
		private string _url;
		private readonly string _tempDzrUrl =  new DzrUrl().ToString();
		public DzrWebValidator(ISettings settings)
		{
			_settings = settings;
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
			Messages = HttpMessagesFactory.Dzzzr();
		}

		public async Task<DzrPage> SendCode(string code, DzrTask task) => GetNextPage(await Messages.GetText(GetUrl(), GetContextSetCode(code, task)));

		public async Task<DzrPage> LogIn()
		{
			await Messages.Response(LogInUrl(), LogInContext());
			return await GetNextPage();
		}

		private DzrPage GetNextPage(string html) => new DzrPage(html);

		public async Task<DzrPage> GetNextPage() => new DzrPage(await Messages.GetText(GetUrl()));

		public bool IsLogOut(DzrPage page) => page == null || page.Type == PageType.NotFound;

		public string GetContextSetCode(string code, DzrTask task) => task?.GetPostForCode(code);

		public string LogInContext() => $@"notags=&action=auth&login={_settings.Game.Login}&password={_settings.Game.Password}";

		public string GetUrl() => GetBaseUrl() + "?" + _tempDzrUrl;

		private string GetBaseUrl() => _url ??= _settings.Web.DefaultUri;

		public string LogInUrl() => GetBaseUrl();
	}
}