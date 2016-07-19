using System.Collections.Generic;
using System.Linq;
using Sanderling.Motor;
using Sanderling.Parse;
using BotEngine.Common;
using Sanderling.ABot.Parse;

namespace Sanderling.ABot.Bot.Task
{
	public class AnomalyEnter : IBotTask
	{
		public Bot bot;

		static public bool AnomalySuitableGeneral(Interface.MemoryStruct.IListEntry scanResult) =>
			scanResult?.CellValueFromColumnHeader("Group")?.RegexMatchSuccessIgnoreCase("combat") ?? false;

		public IEnumerable<IBotTask> Component
		{
			get
			{
				var memoryMeasurementAtTime = bot?.MemoryMeasurementAtTime;
				var memoryMeasurementAccu = bot?.MemoryMeasurementAccu;

				var memoryMeasurement = memoryMeasurementAtTime?.Value;

				var currentManeuverType = memoryMeasurement?.ShipUi?.Indication?.ManeuverType;

				if (!memoryMeasurement.ManeuverStartPossible())
					yield break;

				var probeScannerWindow = memoryMeasurement?.WindowProbeScanner?.FirstOrDefault();

				var scanResultCombatSite =
					probeScannerWindow?.ScanResultView?.Entry?.FirstOrDefault(AnomalySuitableGeneral);

				if (null != scanResultCombatSite)
					yield return scanResultCombatSite.ClickMenuEntryByRegexPattern(bot, ParseStatic.WarpToAt0RegexPattern);
			}
		}

		public MotionParam Motion => null;
	}
}
