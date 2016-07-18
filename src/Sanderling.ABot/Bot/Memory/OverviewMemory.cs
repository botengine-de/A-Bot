using Bib3;
using BotEngine.Interface;
using Sanderling.Parse;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sanderling.ABot.Bot.Memory
{
	public class OverviewMemory
	{
		readonly IDictionary<Int64, HashSet<EWarTypeEnum>> setEWarTypeFromOverviewEntryId = new Dictionary<Int64, HashSet<EWarTypeEnum>>();

		public IEnumerable<EWarTypeEnum> SetEWarTypeFromOverviewEntry(IOverviewEntry entry) =>
			setEWarTypeFromOverviewEntryId?.TryGetValueOrDefault(entry?.Id ?? -1);

		static readonly IEnumerable<ShipManeuverTypeEnum> setManeuverReset =
			new[] { ShipManeuverTypeEnum.Warp, ShipManeuverTypeEnum.Docked, ShipManeuverTypeEnum.Jump };

		public void Aggregate(FromProcessMeasurement<IMemoryMeasurement> memoryMeasurementAtTime)
		{
			var memoryMeasurement = memoryMeasurementAtTime?.Value;

			var overviewWindow = memoryMeasurement?.WindowOverview?.FirstOrDefault();

			foreach (var overviewEntry in (overviewWindow?.ListView?.Entry?.WhereNotDefault()).EmptyIfNull())
			{
				var setEWarType = setEWarTypeFromOverviewEntryId.TryGetValueOrDefault(overviewEntry.Id);

				foreach (var ewarType in overviewEntry.EWarType.EmptyIfNull())
				{
					if (null == setEWarType)
						setEWarType = new HashSet<EWarTypeEnum>();

					setEWarType.Add(ewarType);
				}

				if (null != setEWarType)
					setEWarTypeFromOverviewEntryId[overviewEntry.Id] = setEWarType;
			}

			if (setManeuverReset.Contains(memoryMeasurement?.ShipUi?.Indication?.ManeuverType ?? ShipManeuverTypeEnum.None))
			{
				var setOverviewEntryVisibleId = overviewWindow?.ListView?.Entry?.Select(entry => entry.Id)?.ToArray();

				foreach (var entryToRemoveId in setEWarTypeFromOverviewEntryId.Keys.Where(entryId => !(setOverviewEntryVisibleId?.Contains(entryId) ?? false)).ToArray())
					setEWarTypeFromOverviewEntryId.Remove(entryToRemoveId);
			}
		}
	}
}
