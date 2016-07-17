using System.Collections.Generic;
using System.Linq;
using Sanderling.Motor;
using Sanderling.Parse;
using BotEngine.Common;

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

				if (ShipManeuverTypeEnum.Warp == currentManeuverType ||
					ShipManeuverTypeEnum.Jump == currentManeuverType)
					yield break;

				var probeScannerWindow = memoryMeasurement?.WindowProbeScanner?.FirstOrDefault();

				var scanResultCombatSite =
					probeScannerWindow?.ScanResultView?.Entry?.FirstOrDefault(AnomalySuitableGeneral);

				if (null != scanResultCombatSite)
					yield return new MenuEntryInMenuRootTask
					{
						Bot = bot,
						RootUIElement = scanResultCombatSite,
						MenuEntryRegexPattern = @"warp.*within\s*0",
					};
			}
		}

		public MotionParam Motion => null;
	}
}
