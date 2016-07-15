using System.Collections.Generic;
using Sanderling.Motor;
using Sanderling.Interface.MemoryStruct;
using System.Linq;
using BotEngine.Common;
using Bib3.Geometrik;

namespace Sanderling.ABot.Bot.Task
{
	public class MenuEntryInMenuRootTask : IBotTask
	{
		public Bot Bot;

		public IUIElement RootUIElement;

		public string MenuEntryRegexPattern;

		public IEnumerable<IBotTask> Component => null;

		bool MenuOpenOnRootPossible()
		{
			if (!(Bot.MouseClickLastAgeStepCountFromUIElement(RootUIElement) <= 1))
				return false;

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

				var menu = memoryMeasurement?.Menu?.FirstOrDefault();

				var rootUIElement = RootUIElement;

				if (null == rootUIElement)
					return null;

				var menuEntry = menu?.Entry?.FirstOrDefault(c => c?.Text?.RegexMatchSuccessIgnoreCase(MenuEntryRegexPattern) ?? false);

				if (MenuOpenOnRootPossible() && null != menuEntry)
					return menuEntry?.MouseClick(BotEngine.Motor.MouseButtonIdEnum.Left);

				return RootUIElement?.MouseClick(BotEngine.Motor.MouseButtonIdEnum.Right);
			}
		}
	}
}
