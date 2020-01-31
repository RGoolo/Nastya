namespace Web.Base
{
	public interface IWebValidator
	{
		//ISettings Settings { get; }
		//public GameController Controller { get; }

		//public event SendLightMsgDel SendMsg;
		string GetContextSetCode(string code);// => $"LevelId={_oldPage?.LevelId ?? "1"}&LevelNumber={_oldPage?.LevelNumber ?? "1"}&LevelAction.Answer=" + code;
		string GetContextSetSpoyler(string code);
		string GetUrl();


	}
}
