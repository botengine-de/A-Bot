using BotEngine.Motor;
using Sanderling.Accumulation;
using Sanderling.Motor;
using System.Collections.Generic;
using System.Linq;

namespace Sanderling.ABot.Bot.Task
{
	static public class ModuleTaskExtension
	{
		static public IBotTask EnsureActivated(
			this Bot bot,
			IShipUiModule module)
		{
			if (module?.RampActive ?? false)
				return null;

			if (bot?.MouseClickLastAgeStepCountFromUIElement(module) <= 1)
				return null;

			return new BotTask { Motion = module?.MouseClick(MouseButtonIdEnum.Left) };
		}

		static public IBotTask EnsureActivated(
			this Bot bot,
			IEnumerable<IShipUiModule> setModule) =>
			new BotTask { Component = setModule?.Select(module => bot?.EnsureActivated(module)) };
	}
}
