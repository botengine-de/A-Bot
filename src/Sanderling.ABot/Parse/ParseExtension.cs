using Bib3;
using Bib3.Geometrik;
using BotEngine.Common;
using MemoryStruct = Sanderling.Interface.MemoryStruct;
using Sanderling.Parse;
using System;
using System.Linq;

namespace Sanderling.ABot.Parse
{
	static public class ParseExtension
	{
		static public int? CountFromDroneGroupCaption(this string groupCaption) =>
			groupCaption?.RegexMatchIfSuccess(@"\((\d+)\)")?.Groups[1]?.Value?.TryParseInt();

		/// <summary>
		/// Hobgoblin I ( <color=0xFF00FF00>Idle</color> )
		/// </summary>
		const string StatusStringFromDroneEntryTextRegexPattern = @"\((.*)\)";

		static public string StatusStringFromDroneEntryText(this string droneEntryText) =>
			droneEntryText?.RegexMatchIfSuccess(StatusStringFromDroneEntryTextRegexPattern)?.Groups[1]?.Value?.RemoveXmlTag()?.Trim();

		static public bool ManeuverStartPossible(this IMemoryMeasurement memoryMeasurement) =>
			!(memoryMeasurement?.IsDocked ?? false) &&
			!new[] { ShipManeuverTypeEnum.Warp, ShipManeuverTypeEnum.Jump, ShipManeuverTypeEnum.Docked }.Contains(
				memoryMeasurement?.ShipUi?.Indication?.ManeuverType ?? ShipManeuverTypeEnum.None);

		static public Int64 Width(this RectInt rect) => rect.Side0Length();
		static public Int64 Height(this RectInt rect) => rect.Side1Length();

		static public bool IsScrollable(this MemoryStruct.IScroll scroll) =>
			scroll?.ScrollHandle?.Region.Height() < scroll?.ScrollHandleBound?.Region.Height() - 4;

		static public bool IsNeutralOrEnemy(this MemoryStruct.IChatParticipantEntry participantEntry) =>
			!(participantEntry?.FlagIcon?.Any(flagIcon =>
				new[] { "good standing", "excellent standing", "Pilot is in your (fleet|corporation)", }
				.Any(goodStandingText => flagIcon?.HintText?.RegexMatchSuccessIgnoreCase(goodStandingText) ?? false)) ?? false);
	}
}
