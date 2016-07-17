using BotEngine.Common;
using System.Collections.Generic;
using System.Linq;
using Sanderling.Motor;
using Sanderling.Parse;
using System;
using Sanderling.Interface.MemoryStruct;
using Sanderling.ABot.Parse;
using Bib3;

namespace Sanderling.ABot.Bot.Task
{
	public class CombatTask : IBotTask
	{
		public Bot bot;

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

				var listOverviewEntryToAttack =
					memoryMeasurement?.WindowOverview?.FirstOrDefault()?.ListView?.Entry?.Where(entry => entry?.MainIcon?.Color?.IsRed() ?? false)
					?.OrderBy(entry => entry?.DistanceMax ?? int.MaxValue)
					?.ToArray();

				var targetSelected =
					memoryMeasurement?.Target?.FirstOrDefault(target => target?.IsSelected ?? false);

				var shouldAttackTarget =
					listOverviewEntryToAttack?.Any(entry => entry?.MeActiveTarget ?? false) ?? false;

				var setModuleWeapon =
					memoryMeasurementAccu?.ShipUiModule?.Where(module => module?.TooltipLast?.Value?.IsWeapon ?? false);

				if (null != targetSelected)
					if (shouldAttackTarget)
						yield return bot.EnsureIsActive(setModuleWeapon);
					else
						yield return new MenuEntryInMenuRootTask { Bot = bot, MenuEntryRegexPattern = "unlock", RootUIElement = targetSelected };

				var overviewEntryLockTarget =
					listOverviewEntryToAttack?.FirstOrDefault(entry => !((entry?.MeTargeted ?? false) || (entry?.MeTargeting ?? false)));

				if (null != overviewEntryLockTarget)
					yield return new MenuEntryInMenuRootTask
					{
						Bot = bot,
						RootUIElement = overviewEntryLockTarget,
						MenuEntryRegexPattern = @"^lock\s*target",
					};

				var droneListView = memoryMeasurement?.WindowDroneView?.FirstOrDefault()?.ListView;

				var droneGroupWithNameMatchingPattern = new Func<string, DroneViewEntryGroup>(namePattern =>
					droneListView?.Entry?.OfType<DroneViewEntryGroup>()?.FirstOrDefault(group => group?.LabelTextLargest()?.Text?.RegexMatchSuccessIgnoreCase(namePattern) ?? false));

				var droneGroupInBay = droneGroupWithNameMatchingPattern("bay");
				var droneGroupInLocalSpace = droneGroupWithNameMatchingPattern("local space");

				var droneInBayCount = droneGroupInBay?.Caption?.Text?.CountFromDroneGroupCaption();
				var droneInLocalSpaceCount = droneGroupInLocalSpace?.Caption?.Text?.CountFromDroneGroupCaption();

				//	assuming that local space is bottommost group.
				var setDroneInLocalSpace =
					droneListView?.Entry?.OfType<DroneViewEntryItem>()
					?.Where(drone => droneGroupInLocalSpace?.RegionCenter()?.B < drone?.RegionCenter()?.B)
					?.ToArray();

				var droneInLocalSpaceSetStatus =
					setDroneInLocalSpace?.Select(drone => drone?.LabelText?.Select(label => label?.Text?.StatusStringFromDroneEntryText()))?.ConcatNullable()?.WhereNotDefault()?.Distinct()?.ToArray();

				var droneInLocalSpaceIdle =
					droneInLocalSpaceSetStatus?.Any(droneStatus => droneStatus.RegexMatchSuccessIgnoreCase("idle")) ?? false;

				if (shouldAttackTarget)
				{
					if (0 < droneInBayCount && droneInLocalSpaceCount < 5)
						yield return new MenuEntryInMenuRootTask
						{
							Bot = bot,
							RootUIElement = droneGroupInBay,
							MenuEntryRegexPattern = @"launch",
						};

					if (droneInLocalSpaceIdle)
						yield return new MenuEntryInMenuRootTask
						{
							Bot = bot,
							RootUIElement = droneGroupInLocalSpace,
							MenuEntryRegexPattern = @"engage",
						};
				}
				else
				{
					if (0 < droneInLocalSpaceCount)
						yield return new MenuEntryInMenuRootTask
						{
							Bot = bot,
							RootUIElement = droneGroupInLocalSpace,
							MenuEntryRegexPattern = @"return.*bay",
						};
				}
			}
		}

		public MotionParam Motion => null;
	}
}
