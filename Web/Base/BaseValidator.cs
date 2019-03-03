using System;
using System.Collections.Generic;
using Web.Game.Model;
using Model.Logic.Settings;
using Model.Types.Class;

namespace Web.Base
{
	public abstract class BaseValidator
	{
		public ISettings Settings { get; protected set; }
		public event SendLightMsgDel SendMsg;
		public abstract string GetContextSetCode(string code);// => $"LevelId={_oldPage?.LevelId ?? "1"}&LevelNumber={_oldPage?.LevelNumber ?? "1"}&LevelAction.Answer=" + code;
		public virtual string GetContextSetSpoyler(string code) => null;
		public abstract string GetUrl();

		protected BaseValidator(ISettings setting)
		{
			Settings = setting;
		}

		protected void SendTexttMsg(string message, Guid? replaceMsgId = null, bool withHtml = false)
		{
			var t = CommandMessage.GetTextMsg(message, withHtmlTags: withHtml);
			t.OnIdMessage = replaceMsgId.GetValueOrDefault();

			var msg = new List<CommandMessage>
			{
				t,
			};
			SendMsg?.Invoke(msg);
		}

		protected void SndMsg(IEnumerable<CommandMessage> messages)
		{
			SendMsg?.Invoke(messages);
		}

		public abstract void AfterSendCode(string html, string code, Guid? idMsg);
		public abstract void SetNewPage(string html);

		public abstract string LogInContext();// => $@"socialAssign=0&Login={Settings.GetValue(Const.Game.Login)}&Password={Const.Game.Password}&EnButton1=Sign+In&ddlNetwork=1";
		public abstract string LogInUrl();//=> $@"{_setting.Site()}Login.aspx";

		public virtual void SendEvent(IEvent iEvent) { }
	}
}
