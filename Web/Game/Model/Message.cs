using Model.Logic.Coordinates;
using Model.Types.Interfaces;
using System;

namespace Web.Game.Model
{
	public enum TypeMsg
	{
		Text, Photo, Coord
	}

	public class Text : global::Model.Types.Class.CommandMessage
	{
		public TypeMsg Type => TypeMsg.Text;
		public bool CheckCoord { get; }
		public string Body { get; set; }
		public Guid ReplaceMSGID { get; }
		public bool WithHtmlTag { get; }
		public string Parametr => null;
		public Text(string body, bool checkCoord = false, Guid? replaceMSGID = null, bool withHtml = false) : base(body)
		{
			CheckCoord = checkCoord;
			Body = body;
			ReplaceMSGID = replaceMSGID ?? Guid.Empty;
			WithHtmlTag = withHtml;
			CheckedCoordsInText = checkCoord;
			OnIdMessage = replaceMSGID ?? Guid.Empty;
			WithHtmlTags = withHtml;
			//if (replaceMSGID.GetValueOrDefault() != Guid.Empty) IsAnswer = true;
		}
		//public CommandMessage CommandMessage { get; private set; }
	}

	public class MessageCoord : global::Model.Types.Class.CommandMessage
	{
		public TypeMsg Type => TypeMsg.Coord;
		public bool CheckCoord { get; }
		public string Body { get; set; }
		public int ReplaceMSGID { get; }
		public bool WithHtmlTag { get; }
		public string Parametr => null;
		public Coordinate coord { get; }

		public MessageCoord(Coordinate coord) : base(coord)
		{
			Coord = coord;
			//this.TypeMessage = Model.Types.Enums.NeedResource,
		}

		//public CommandMessage CommandMessage { get; private set; }
	}

	class Photo : global::Model.Types.Class.CommandMessage
	{
		public TypeMsg Type => TypeMsg.Photo;
		public bool CheckCoord { get; }
		public string Parameter { get; }
		//url image
		//public string Body { get; set; }
		public Guid ReplaceMSGID { get; }
		public bool WithHtmlTag => false;
		public Photo(string body, bool checkCoord = false, Guid? replaceMsgId = null, string parameter = null) : base(true,  new UrlFileToken(body), parameter)
		{
			CheckCoord = checkCoord;
			//Body = body;
			ReplaceMSGID = replaceMsgId ?? Guid.Empty;
			Parameter = parameter;
		
			OnIdMessage = replaceMsgId ?? Guid.Empty;
			//if (replaceMSGID.GetValueOrDefault() != Guid.Empty) IsAnswer = true;
		}
	}
}
