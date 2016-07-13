using Sanderling.Motor;

namespace Sanderling.ABot.Bot
{
	public class MotionRecommendation
	{
		public int Id;

		public MotionParam MotionParam;
	}

	public class BotStepResult
	{
		public MotionRecommendation[] ListMotion;
	}
}
