using System.Collections.Generic;
using Sanderling.Motor;

namespace Sanderling.ABot.Bot.Task
{
	public class DiagnosticTask : IBotTask
	{
		public IEnumerable<IBotTask> Component => null;

		public IEnumerable<MotionParam> Effects => null;

		public string MessageText;
	}
}
