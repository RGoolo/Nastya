using System;
using System.Net;
using System.IO;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using System.Collections.Generic;
using Web.Game.Model;
using System.Collections.Concurrent;
using Model.Logic.Model;
using Model.Logic.Settings;
using System.Linq;
using Model.Types.Class;
using Web.Game;
using Model.Types.Interfaces;

namespace Web.Base
{
	//public delegate void SendMsgSyncDel(IEnumerable<IMessage> messages, long chatId);
	public abstract class BaseGame<T> : IGame where T: BaseValidator 
	{
		public event SendMsgSyncDel SendMsg;

		protected TypeGame TypeGame;
		private CookieContainer _cookies = new CookieContainer();
		private readonly Timer _refreshTimer;// = new Timer(5000);
		private readonly ConcurrentQueue<IEvent> _queue = new ConcurrentQueue<IEvent>();
		private System.Threading.CancellationTokenSource _source;
		private System.Threading.CancellationToken _token;
		protected Encoding Encoding { get; set; } = Encoding.UTF8;

		protected T Validator;// { get; set; }

		protected abstract bool LogIn();
		protected abstract bool IsLogOut(HttpWebResponse response);

		private void Cycle()
		{
			while (true)
			{
				if (_queue.IsEmpty) continue;
				if (!_queue.TryDequeue(out var iEvent)) continue;

				try
				{
					ThreadSaveEvent(iEvent);
				}
				catch (GameException ex)
				{
					try
					{
						SendSimpleMsg(ex.Message, iEvent.IdMsg);
					}
					catch
					{
						// ignored
					}
				}
				catch (Exception ex)
				{
					try
					{
						SendSimpleMsg("Что-то пошло не так!", iEvent.IdMsg);
						Console.WriteLine(ex.Message, ex.StackTrace);
					}
					catch
					{
						// ignored
					}
				}
			}
		}

		protected virtual void ThreadSaveEvent(IEvent iEvent)
		{
			switch (iEvent.EventType)
			{
				case EventTypes.Refresh:
					Refresh();
					break;

				case EventTypes.StartGame:
					Start();
					break;

				case EventTypes.StopGame:
					Stop();
					break;

				case EventTypes.SendCode:
					var strcontext = Validator.GetContextSetCode(iEvent.Text);
					var page = GetPage(PostHttpWebRequest(Validator.GetUrl(), strcontext));
					Validator.AfterSendCode(page, iEvent.User, iEvent.Text, iEvent.IdMsg);
					break;

				case EventTypes.SendSpoiler:
					var context = Validator.GetContextSetSpoyler(iEvent.Text);
					if (context == null)
						throw new GameException("Не найти спойлер.");

					var page2 = GetPage(PostHttpWebRequest(Validator.GetUrl(), context));
					Validator.AfterSendCode(page2, iEvent.User, iEvent.Text, iEvent.IdMsg);
					break;

				default:
					//Validator.l
					Validator.SendEvent(iEvent);
					break;
			}
		}

		public void SetEvent(IEvent iEvent)
		{
			if (iEvent.EventType == EventTypes.StartGame)
			{
				_queue.Clear();
				Validator.Settings.SetValue(Const.Game.GameIsStart, Start().ToString());
			}
			else
			{
				_queue.Enqueue(iEvent);
			}

		}

		protected BaseGame(T baseValidator, TypeGame typeGame)
		{
			TypeGame = typeGame;
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
			_source = new System.Threading.CancellationTokenSource();
			_token = _source.Token;
			Task.Run(() => Cycle(), _token);
			Validator = baseValidator;
			Validator.SendMsg += _validator_SendMsg;
			_refreshTimer = new Timer(3000);
			_refreshTimer.Elapsed += _refreshTimer_Elapsed;
		}

		private void _validator_SendMsg(IEnumerable<CommandMessage> messages)
		{
			SendMsg?.Invoke(messages.ToList(), Validator.Settings.ChatGuid);
		}

		private void _validator_SendMsg(CommandMessage message) => _validator_SendMsg(new List<CommandMessage>() { message });


		protected void SendSimpleMsg(string s, Guid? idmsg = null)
		{
			var msg = CommandMessage.GetTextMsg(s);
			msg.OnIdMessage = idmsg.GetValueOrDefault();


			_validator_SendMsg(msg);
		}

		private void _refreshTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			//Много паранойи
			if (_queue.IsEmpty)
				_queue.Enqueue(new SimpleEvent(EventTypes.Refresh, null));
		}

		protected virtual void Stop()
		{
			_refreshTimer.Stop();
			_source.Cancel();
			_cookies = new CookieContainer();
			Validator.Settings.SetValue(Const.Game.GameIsStart, false.ToString());
		}

		protected virtual bool Start()
		{
			if (!((TypeGame & TypeGame.Dummy) == TypeGame.Dummy) && !LogIn())
			{
				SendSimpleMsg("Не удалось подключиться к серверу, проверте Логин, Пароль и Ссылку на игру.");
				return false;
			}

			SendSimpleMsg("Успешно подключилась.");

			_source = new System.Threading.CancellationTokenSource();
			_token = _source.Token;

			_refreshTimer.Start();
			Refresh();
			return true;
		}

		protected virtual HttpWebRequest AddCustomHeaders(HttpWebRequest request)
		{
			return request;
		}

		protected HttpWebRequest GetWebRequest(string url) => AddCustomHeaders(Helper.GetWebRequest(url, _cookies));

		protected HttpWebResponse GetResponse(HttpWebRequest request)
		{
			try
			{
				var resqSite = (HttpWebResponse)request.GetResponse();
				_cookies = Helper.GetCookies(resqSite.Headers, request.CookieContainer);
				return resqSite;
			}
			catch
			{

			}
			return null;
		}

		protected string GetHtml(HttpWebRequest request)
		{
			if ((TypeGame & TypeGame.Dummy) == TypeGame.Dummy)
				return WebHelper.GetTestPage(Validator.Settings.Web.Domen);

			return Helper.GetHTML(request, ref _cookies, Encoding);
		}

		protected HttpWebRequest PostHttpWebRequest(string url, string context) => AddCustomHeaders(Helper.PostHttpWebRequest(url, context, _cookies));

		protected string GetPage(HttpWebRequest request)
		{
			if ((TypeGame & TypeGame.Dummy) == TypeGame.Dummy)
				return WebHelper.GetTestPage(Validator.Settings.Web.Domen);

			var response = GetResponse(request);
			if (response == null)
				return null;

			if (IsLogOut(response))
				if (!LogIn())
					SendSimpleMsg("Отвалилось подключение");

			
			return new StreamReader(response.GetResponseStream(), Encoding).ReadToEnd();
		}

		protected void AddzrCoocies()
		{
			try
			{
				_cookies.Add(new Cookie("b", "b", "/", ".dzzzr.ru"));//ToDo: Domen ".dzzzr.ru"
				_cookies.Add(new Cookie("hotlog", "1", "/", ".dzzzr.ru"));//ToDo: Domen ".dzzzr.ru"
			}

			catch (Exception ex) {/* Console.WriteLine(ex.Message);*/ }; //ToDo
		}

		protected virtual void Refresh()
		{
			try
			{
				string page = null;

				page = (TypeGame & TypeGame.Dummy) == TypeGame.Dummy ? WebHelper.GetTestPage(Validator.Settings.Web.Domen) : GetPage(GetWebRequest(Validator.GetUrl()));

				Validator.SetNewPage(page);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message + ex.StackTrace);
			}
		}

		public void Dispose() => Stop();
		public virtual void SendCode(string str, IUser user, Guid replaceMsg)
		{
			if (string.IsNullOrEmpty(str))
				return;

			if (str.StartsWith("."))
				SetEvent(new SimpleEvent(EventTypes.SendCode, user, str.Substring(1), replaceMsg));
		}

		public void SendCode(string v, object p)
		{
			throw new NotImplementedException();
		}

		public enum TypeCode
		{
			code, spoyler
		}
	}
}
