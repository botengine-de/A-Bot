using Bib3;
using BotEngine.Common;
using Sanderling.Parse;
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
	}
}
