using Sanderling.Motor;
using System;

namespace Sanderling.ABot.Bot
{
	public class MotionRecommendation
	{
		public int Id;

		public MotionParam MotionParam;
	}

	public class BotStepResult
	{
		public Exception Exception;

		public MotionRecommendation[] ListMotion;

		public IBotTask[][] OutputListTaskPath;
	}
}
