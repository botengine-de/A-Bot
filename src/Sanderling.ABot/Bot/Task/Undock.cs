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

		public IEnumerable<MotionParam> Effects
		{
			get
			{
				if (MemoryMeasurement?.IsUnDocking ?? false)
					yield break;

				if (!(MemoryMeasurement?.IsDocked ?? false))
					yield break;

				yield return MemoryMeasurement?.WindowStation?.FirstOrDefault()?.UndockButton?.MouseClick(BotEngine.Motor.MouseButtonIdEnum.Left);
			}
		}
	}
}
