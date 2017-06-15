using Sanderling.Motor;
using System.Collections.Generic;

namespace Sanderling.ABot.Bot
{
	public interface IBotTask
	{
		IEnumerable<IBotTask> Component { get; }

		/// <summary>
		/// Effects to apply to the eve online client.
		/// </summary>
		IEnumerable<MotionParam> Effects { get; }
	}

	public class BotTask : IBotTask
	{
		public IEnumerable<IBotTask> Component { set; get; }

		public IEnumerable<MotionParam> Effects { set; get; }
	}
}
