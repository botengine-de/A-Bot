using Bib3;
using BotEngine.Common;
using Sanderling.ABot.Bot.Task;
using Sanderling.Interface.MemoryStruct;
using Sanderling.Parse;
using System.Collections.Generic;
using System.Linq;

namespace Sanderling.ABot.Bot
{
	static public class BotExtension
	{
		static readonly EWarTypeEnum[][] listEWarPriorityGroup = new[]
		{
			new[] { EWarTypeEnum.ECM },
			new[] { EWarTypeEnum.Web},
			new[] { EWarTypeEnum.WarpDisrupt, EWarTypeEnum.WarpScramble },
		};

		static public int AttackPriorityIndexForOverviewEntryEWar(IEnumerable<EWarTypeEnum> setEWar)
		{
			var setEWarRendered = setEWar?.ToArray();

			return
				listEWarPriorityGroup.FirstIndexOrNull(priorityGroup => priorityGroup.ContainsAny(setEWarRendered)) ??
				(listEWarPriorityGroup.Length + (0 < setEWarRendered?.Length ? 0 : 1));
		}

		static public int AttackPriorityIndex(
			this Bot bot,
			Sanderling.Parse.IOverviewEntry entry) =>
			AttackPriorityIndexForOverviewEntryEWar(bot?.OverviewMemory?.SetEWarTypeFromOverviewEntry(entry));

		static public bool ShouldBeIncludedInStepOutput(this IBotTask task) =>
			null != task?.Motion || task is DiagnosticTask;

		static public bool LastHasMotion(this IEnumerable<IBotTask> listTask) =>
			null != listTask?.LastOrDefault()?.Motion;

		static public IEnumerable<IBotTask[]> TakeSubsequenceWhileUnwantedInferenceRuledOut(this IEnumerable<IBotTask[]> listTaskPath) =>
			listTaskPath
			?.EnumerateSubsequencesStartingWithFirstElement()
			?.OrderBy(subsequenceTaskPath => 1 == subsequenceTaskPath?.Count(BotExtension.LastHasMotion))
			?.LastOrDefault();

		static public IUIElementText TitleElementText(this IModuleButtonTooltip tooltip) =>
			tooltip?.LabelText?.OrderByCenterVerticalDown()?.FirstOrDefault();

		static public bool ShouldBeActivePermanent(this Accumulation.IShipUiModule module, Bot bot) =>
			new[]
			{
				module?.TooltipLast?.Value?.IsHardener,
				bot?.ConfigSerialAndStruct.Value?.ModuleActivePermanentSetTitlePattern
					?.Any(activePermanentTitlePattern => module?.TooltipLast?.Value?.TitleElementText()?.Text?.RegexMatchSuccessIgnoreCase(activePermanentTitlePattern) ?? false),
			}
			.Any(sufficientCondition => sufficientCondition ?? false);
	}
}
