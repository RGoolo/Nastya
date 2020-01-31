using System;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using System.Collections.Generic;
using Web.Game.Model;
using System.Collections.Concurrent;
using Model.Logic.Model;
using Model.Logic.Settings;
using Model.BotTypes.Class;
using Model.BotTypes.Enums;
using Model.BotTypes.Interfaces;
using Model.BotTypes.Interfaces.Messages;
using Model.Logger;

namespace Web.Base
{
	//public delegate void SendMsgSyncDel(IEnumerable<IMessage> messages, long chatId);
	public class ConcurrentGame : IGame
	{
		public event SendMsgsSyncDel SendMsgs;

		ILogger logger = Logger.CreateLogger(nameof(ConcurrentGame));
		private readonly Timer _refreshTimer;// = new Timer(5000);
		private readonly ConcurrentQueue<IEvent> _queue = new ConcurrentQueue<IEvent>();
		
		private bool _gameIsStarted;
		private bool _refreshPage;
		private readonly System.Threading.CancellationTokenSource _source;
		private readonly System.Threading.CancellationToken _token;
		private readonly IController _controller;
		private readonly ISendSyncMsgs _sendSyncMessage;
		private readonly object _locker = new object();
		
		private void Cycle()
		{
			if (!TryStart()) return;

			while (_gameIsStarted)
			{
				IEvent iEvent = null;

				try
				{
					if (_queue.IsEmpty)
					{
						if (_refreshPage)
						{
							_controller.Refresh();
							_refreshPage = false;
						}
						continue;
					}

					if (!_queue.TryDequeue(out iEvent)) continue;

					_controller.SendEvent(iEvent);
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

		public void SetEvent(IEvent iEvent) => _queue.Enqueue(iEvent);
	
		public ConcurrentGame(IController concreteGame, ISendSyncMsgs sendSyncMessage)
		{
			_controller = concreteGame;
			_sendSyncMessage = sendSyncMessage;

			concreteGame.SendMsgs += ConcreteGame_SendMsgs; ;

			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
			_source = new System.Threading.CancellationTokenSource();
			_token = _source.Token;

			_refreshTimer = new Timer(3000);
			_refreshTimer.Elapsed += _refreshTimer_Elapsed;
		}

		private bool TryStart()
		{
			try
			{
				_controller.LogIn();
				/*
				if (_controller.IsLogOut()) //ToDo try  AutorizationExc
				{
					var msgError = CommandMessage.GetTextMsg($"Не удалось подключиться. Проверте логин и пароль. login: { _controller.Settings.Game.Login}");

					SendSimpleMsg(msgError);


					_gameIsStarted = false;
					return false;
				}*/

				var msg = MessageToBot.GetTextMsg("Успешно подключилась.");
				msg.Notification = Notification.GameStarted;
				SendSimpleMsg(msg);

				_gameIsStarted = true;
				//Task.Run(Cycle, _token);

				_refreshTimer.Start();
			}
			catch (AuthorizationFailedException ex)
			{
				logger.Error(ex);
				_gameIsStarted = false;
				var msgError = MessageToBot.GetTextMsg(ex.Message);
				SendSimpleMsg(msgError);

				return false;
			}
			catch (Exception ex)
			{
				logger.Error(ex);
				
				_gameIsStarted = false;

				//ToFo gameException?
				var msgError = MessageToBot.GetTextMsg("Произошла ошибка подключения.");
				SendSimpleMsg(msgError);
				SendSimpleMsg(ex.Message);

				return false;
				//ToDo: log;
			}

			return true;
		}

		public async Task Start()
		{
			if (_gameIsStarted)
			{
				var msgError = MessageToBot.GetTextMsg("Игра уже запущена.");
				SendSimpleMsg(msgError);
				return;
			}

			_gameIsStarted = true;
			await Task.Run(Cycle, _token);
		}

		private void ConcreteGame_SendMsgs(IEnumerable<IMessageToBot> messages) => SendSimpleMsg(messages);

		private void _refreshTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			if (_queue.IsEmpty)
				_refreshPage = true;
		}

		protected void SendSimpleMsg(string s, IMessageId idMsg = null)
		{
			var msg = MessageToBot.GetTextMsg(s);
			msg.OnIdMessage = idMsg;
			SendSimpleMsg(msg);
		}

		protected void SendSimpleMsg(IMessageToBot msg) => SendSimpleMsg(new List<IMessageToBot>() { msg });

		protected void SendSimpleMsg(IEnumerable<IMessageToBot> msgs) => _sendSyncMessage.SendSync(msgs);
		
		public void Stop()
		{
			lock (_locker)
			{
				_gameIsStarted = false;

				var msg = MessageToBot.GetTextMsg("Перестаю следить за игрой.");
				msg.Notification = Notification.GameStoped;
				SendSimpleMsg(msg);
			}
		}

		public void Dispose() => Stop();

		public void SendCode(string code, IUser user, IMessageId replaceMsg)
		{
			var iEvents = _controller.GetCode(code, user, replaceMsg);
			if (iEvents == null)
				return;

			foreach (var iEvent in iEvents)
				SetEvent(iEvent);
		}
	}
}
