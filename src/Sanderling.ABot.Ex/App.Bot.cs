using Bib3.Synchronization;

namespace Sanderling.ABot.Exe
{
	partial class App
	{
		readonly object botLock = new object();

		readonly Bot.Bot bot = new Bot.Bot();

		void BotProgress()
		{
			botLock.InvokeIfNotLocked(() =>
			{
				var memoryMeasurementLast = this.MemoryMeasurementLast;

				var time = memoryMeasurementLast?.End;

				if (!time.HasValue)
					return;

				if (time <= bot?.StepLastInput?.TimeMilli)
					return;

				bot.Step(new Bot.BotStepInput
				{
					TimeMilli = time.Value,
					FromProcessMemoryMeasurement = memoryMeasurementLast,
				});
			});
		}
	}
}
