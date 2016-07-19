using System.Collections.Generic;
using Sanderling.Motor;
using Sanderling.Interface.MemoryStruct;
using System.Linq;
using BotEngine.Common;
using Bib3.Geometrik;
using System;
using Bib3;

namespace Sanderling.ABot.Bot.Task
{
	public class MenuPathTask : IBotTask
	{
		public Bot Bot;

		public IUIElement RootUIElement;

		public string[][] ListMenuListPriorityEntryRegexPattern;

		public IEnumerable<IBotTask> Component => null;

		bool MenuOpenOnRootPossible()
		{
			var memoryMeasurement = Bot?.MemoryMeasurementAtTime?.Value;

			var menu = memoryMeasurement?.Menu?.FirstOrDefault();

			if (null == menu)
				return false;

			var overviewEntry = RootUIElement as IOverviewEntry;

			IUIElement regionExpected = RootUIElement;

			if (null != overviewEntry)
			{
				regionExpected = memoryMeasurement?.WindowOverview?.FirstOrDefault();

				if (!(overviewEntry.IsSelected ?? false))
					return false;

				if (!(menu?.Entry?.Any(menuEntry => menuEntry?.Text?.RegexMatchSuccessIgnoreCase(@"remove.*overview") ?? false) ?? false))
					return false;
			}

			if (regionExpected.Region.Intersection(menu.Region.WithSizeExpandedPivotAtCenter(10)).IsEmpty())
				return false;

			return true;
		}

		public MotionParam Motion
		{
			get
			{
				var memoryMeasurement = Bot?.MemoryMeasurementAtTime?.Value;

				var listMenu = memoryMeasurement?.Menu?.ToArray();

				var rootUIElement = RootUIElement;

				if (null == rootUIElement)
					return null;

				IMenuEntry menuEntryToContinue = null;

				var mouseClickOnRootAge = Bot?.MouseClickLastAgeStepCountFromUIElement(RootUIElement);

				if (MenuOpenOnRootPossible() && mouseClickOnRootAge <= listMenu?.Length)
				{
					var levelCount = Math.Min(ListMenuListPriorityEntryRegexPattern?.Length ?? 0, listMenu?.Length ?? 0);

					for (int levelIndex = 0; levelIndex < levelCount; levelIndex++)
					{
						var listPriorityEntryRegexPattern = ListMenuListPriorityEntryRegexPattern[levelIndex];

						var menuEntry =
							listPriorityEntryRegexPattern
							?.Select(priorityEntryRegexPattern =>
								listMenu[levelIndex]?.Entry
								?.FirstOrDefault(c => c?.Text?.RegexMatchSuccessIgnoreCase(priorityEntryRegexPattern) ?? false))
							?.WhereNotDefault()?.FirstOrDefault();

						if (null == menuEntry)
							break;

						menuEntryToContinue = menuEntry;

						if (!(menuEntry?.HighlightVisible ?? false))
							break;
					}
				}

				return
					menuEntryToContinue?.MouseClick(BotEngine.Motor.MouseButtonIdEnum.Left) ??
					RootUIElement?.MouseClick(BotEngine.Motor.MouseButtonIdEnum.Right);
			}
		}
	}

	static public class MenuTaskExtension
	{
		static public MenuPathTask ClickMenuEntryByRegexPattern(
			this IUIElement rootUIElement,
			Bot bot,
			string menuEntryRegexPattern)
		{
			if (null == rootUIElement)
				return null;

			return new MenuPathTask
			{
				Bot = bot,
				RootUIElement = rootUIElement,
				ListMenuListPriorityEntryRegexPattern = new[] { new[] { menuEntryRegexPattern } },
			};
		}
	}
}
