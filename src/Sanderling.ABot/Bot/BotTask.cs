using Sanderling.Motor;
using System.Collections.Generic;

namespace Sanderling.ABot.Bot
{
	public interface IBotTask
	{
		IEnumerable<IBotTask> Component { get; }

		MotionParam Motion { get; }
	}

	public class BotTask : IBotTask
	{
		public IEnumerable<IBotTask> Component { set; get; }

		public MotionParam Motion { set; get; }
	}
}
