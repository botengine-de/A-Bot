using System.Collections.Generic;
using System.Linq;
using Sanderling.Motor;
using Sanderling.Parse;

namespace Sanderling.ABot.Bot.Task
{
	public class UndockTask : IBotTask
	{
		public IMemoryMeasurement MemoryMeasurement;

		public IEnumerable<IBotTask> Component => null;

		public MotionParam Motion
		{
			get
			{
				if (MemoryMeasurement?.IsUnDocking ?? false)
					return null;

				if (!(MemoryMeasurement?.IsDocked ?? false))
					return null;

				return MemoryMeasurement?.WindowStation?.FirstOrDefault()?.UndockButton?.MouseClick(BotEngine.Motor.MouseButtonIdEnum.Left);
			}
		}
	}
}
