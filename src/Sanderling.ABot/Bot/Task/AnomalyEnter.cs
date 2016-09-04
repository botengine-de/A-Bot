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

        public bool AnomalySuitableForBotConfig(Interface.MemoryStruct.IListEntry scanResult)
        {
            var setAnomalyEnabledNamePattern = bot.ConfigSerialAndStruct.Value?.SetAnomalyEnabledNamePattern;
            var anomalyName = scanResult?.CellValueFromColumnHeader("Name");

            return
                setAnomalyEnabledNamePattern?.Any(enabledNamePattern =>
                anomalyName?.RegexMatchSuccessIgnoreCase(enabledNamePattern) ?? false) ?? true;
        }

        public bool AnomalySuitable(Interface.MemoryStruct.IListEntry scanResult) =>
            AnomalySuitableGeneral(scanResult) &&
            AnomalySuitableForBotConfig(scanResult);

        public IEnumerable<IBotTask> Component
        {
            get
            {
                var memoryMeasurementAtTime = bot?.MemoryMeasurementAtTime;
                var memoryMeasurementAccu = bot?.MemoryMeasurementAccu;

                var memoryMeasurement = memoryMeasurementAtTime?.Value;

                if (!memoryMeasurement.ManeuverStartPossible())
                    yield break;

                var probeScannerWindow = memoryMeasurement?.WindowProbeScanner?.FirstOrDefault();

                var scanResultCombatSite =
                    probeScannerWindow?.ScanResultView?.Entry?.FirstOrDefault(AnomalySuitable);

                if (null != scanResultCombatSite)
                    yield return scanResultCombatSite.ClickMenuEntryByRegexPattern(bot, ParseStatic.MenuEntryWarpToAtLeafRegexPattern);
            }
        }

        public MotionParam Motion => null;
    }
}
