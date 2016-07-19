using System.Collections.Generic;
using Sanderling.Motor;
using BotEngine.Motor;
using Sanderling.Parse;

namespace Sanderling.ABot.Bot.Task
{
	public class EnableInfoPanelCurrentSystem : IBotTask
	{
		public IMemoryMeasurement MemoryMeasurement;

		public IEnumerable<IBotTask> Component => null;

		public MotionParam Motion
		{
			get
			{
				if (null != MemoryMeasurement?.InfoPanelCurrentSystem)
					return null;

				return MemoryMeasurement?.InfoPanelButtonCurrentSystem?.MouseClick(MouseButtonIdEnum.Left);
			}
		}
	}
}
