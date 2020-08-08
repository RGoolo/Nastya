using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Model.Bots.BotTypes.Attribute;
using Model.Bots.BotTypes.Class;
using Model.Bots.BotTypes.Enums;
using Model.Bots.BotTypes.Interfaces.Messages;
using Model.Bots.TelegramBot.HtmlParse;
using Model.Files.FileTokens;
using Model.Logic.Coordinates;
using Model.Logic.Settings;

namespace Nastya.Commands
{

	//Display(Description = "Работа с сообщениями", Name = "coords"),

	[CommandClass("coords", "Работа с координатами.", TypeUser.User)]
	public class CoordinatesCommand
	{
		private ISettings Settings { get; }

		public CoordinatesCommand(ISettings settings)
		{
			Settings = settings;

			_pointsFactory = settings.PointsFactory;
		}

		private IPointsFactory _pointsFactory;
		private string _googleCreds;

		[Command(nameof(CheckCoordintate), "Проверять сообщения на координаты.")]
		public bool CheckCoordintate { get; set; }

		[Command(nameof(AddPicture), "Добавлять картинку координат, при построении маршрутов")]
		public bool AddPicture { get; set; }

		[CommandOnMsg(nameof(CheckCoordintate), MessageType.Text, typeUser: TypeUser.User)]
		public List<IMessageToBot> GetAllCoordinates(IBotMessage msg)
		{
			var result = new List<IMessageToBot>(_pointsFactory.GetCoordinates(msg.Text).Points().Select(CommandMessageWithDescription));
            return result;
        }

		[CommandOnMsg(nameof(CheckCoordintate), MessageType.Text, typeUser: TypeUser.Bot)]
		public async Task<TransactionCommandMessage> GetAllCoordinatesForBot(IBotMessage msg)
		{
			var result = new List<IMessageToBot>();
			if (string.IsNullOrEmpty(msg.Text))
				return null;

			var text = TelegramHtml.RemoveAllTag(msg.Text);
			var points = _pointsFactory.GetCoordinates(text).Points();
			if (points.Count == 0)
				return null;

			result.AddRange(points.Select(CommandMessageWithDescription));
		
			if (AddPicture && points.Count > 1)
			{
				var file = Settings.FileChatFactory.NewResourcesFileByExt(".jpg");
				await _pointsFactory.SetPicture(file, points);
				
				var textImg = string.Join("\n", points.Select(p => $"{p.Alias}) {p}"));
				result.Add(MessageToBot.GetPhototMsg(file, (Texter)textImg));
			}
			
			return new TransactionCommandMessage(result);
		}

		private IMessageToBot CommandMessageWithDescription(Coordinate coord) =>
			MessageToBot.GetCoordMsg(coord, coord.OriginText);// _pointsFactory.GetUrlLink(coord, true));
		
		[Command(nameof(Coords), "Распознать координаты из сообщения.")]
		public IList<IMessageToBot> Coords(IBotMessage msg) => 
			GetCoordinate(msg, nameof(Coords), (x) => _pointsFactory.GetCoordinates(x).Points());

		//[Command(nameof(Places), "Скинуть  из текста.")]
//		public TransactionCommandMessage Places(IMessage msg) =>
//			GetCoordinate(msg, null, _pointsFactory.GetPlaces);

		[Command(nameof(Route), "Построить маршрут по координатам из текста.")]
		public Task<IList<IMessageToBot>> Route(IBotMessage msg, IChatFileFactory fWorker) =>  
			GetCommand(msg, nameof(Route), _pointsFactory.GetCoordinates, fWorker, !AddPicture);

		[Command(nameof(Places), "Построить маршрут, по местам, разделлеными новой строкой или ';'")]
		public Task<IList<IMessageToBot>> Places(IBotMessage msg, IChatFileFactory fWorker) =>
			GetCommand(msg, nameof(Places), (x) => _pointsFactory.GetPlaces(x), fWorker, AddPicture);

	
		[Command(nameof(AddTextCoord), "Добавить построчно ссылки.")]
		public Task<IList<IMessageToBot>> AddTextCoord(IBotMessage msg, IChatFileFactory fWorker) => 
			GetCommand(msg,  "/" + nameof(AddTextCoord), _pointsFactory.GetCoordinates, fWorker);

		private string GetText(IBotMessage msg, string fName)
		{
			return (msg.ReplyToMessage != null) 
				? msg.ReplyToMessage.Text 
				: string.IsNullOrEmpty(fName) ? msg.Text : msg.Text.Replace("/" + fName, "", true, CultureInfo.InvariantCulture);
		}
		
		private async Task<IList<IMessageToBot>> GetCommand<T>(IBotMessage msg, string fName, Func<string, PointWorker<T>> func, IChatFileFactory fWorker, bool addPicture = false) where T: Point
		{
			var text = GetText(msg, fName);
			Console.WriteLine("text:" + text);
            IList<IMessageToBot> result = new List<IMessageToBot>();
			var pointWorker = func(text);
			//if (isText.HasValue)
			{
				//var file = SettingsHelper.GetSetting(ChatId).FileWorker.NewFileTokenByExt(".jpg");

				var file = fWorker.NewResourcesFileByExt(".jpg");
				//if (addPicture) _pointsFactory.GetPictureText(text, file);
				await _pointsFactory.SetPicture(file, pointWorker.Points());

                var tomsg = MessageToBot.GetHTMLPhototMsg(file, "");
                tomsg.OnIdMessage = msg.MessageId;
				result.Add(tomsg);
			}
			//else
			{
				//result = CommandMessage.GetTextMsg(new Texter(func(text), true));
			}
            var tomsg1 = MessageToBot.GetTextMsg(new Texter( pointWorker.TotalPoints(), true, false));
            tomsg1.OnIdMessage = msg.MessageId;
            result.Add(tomsg1);
			return result;
		}

		private IList<IMessageToBot> GetCoordinate(IBotMessage msg, string fName, Func<string, IEnumerable<Coordinate>> func)
		{
			var text = GetText(msg, fName);
			var result = new List<IMessageToBot>();

			var coords = func(text);

			coords.ToList().ForEach(x =>
			{
				result.Add(MessageToBot.GetTextMsg((Texter)(x.OriginText + Environment.NewLine))); //+ _pointsFactory.GetUrlLink(x, true)));
				result.Add(MessageToBot.GetCoordMsg(x, x.OriginText));
			});
            return result;
        }

	}
}
