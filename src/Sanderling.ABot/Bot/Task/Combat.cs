using BotEngine.Common;
using System.Collections.Generic;
using System.Linq;
using Sanderling.Motor;
using Sanderling.Parse;

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

				if (null == overviewEntryLockTarget)
					yield break;

				yield return new MenuEntryInMenuRootTask
				{
					Bot = bot,
					RootUIElement = overviewEntryLockTarget,
					MenuEntryRegexPattern = @"^lock\s*target",
				};
			}
		}

		public MotionParam Motion => null;
	}
}
