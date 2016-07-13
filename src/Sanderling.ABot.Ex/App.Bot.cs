using Bib3;
using Bib3.Synchronization;
using BotEngine.Interface;
using Sanderling.Interface.MemoryStruct;
using Sanderling.Motor;
using System.Collections.Generic;
using System.Threading;

namespace Sanderling.ABot.Exe
{
	partial class App
	{
		readonly object botLock = new object();

		readonly Bot.Bot bot = new Bot.Bot();

		const int FromMotionToMeasurementDelayMilli = 300;

		Bot.MotionResult[] BotStepLastMotionResult;

		void BotProgress(bool motionEnable)
		{
			botLock.InvokeIfNotLocked(() =>
			{
				var memoryMeasurementLast = this.MemoryMeasurementLast;

				var time = memoryMeasurementLast?.End;

				if (!time.HasValue)
					return;

				if (time <= bot?.StepLastInput?.TimeMilli)
					return;

				var stepResult = bot.Step(new Bot.BotStepInput
				{
					TimeMilli = time.Value,
					FromProcessMemoryMeasurement = memoryMeasurementLast,
					StepLastMotionResult = BotStepLastMotionResult,
				});

				if (motionEnable)
					BotMotion(memoryMeasurementLast, stepResult?.ListMotion);
			});
		}

		void BotMotion(
			FromProcessMeasurement<IMemoryMeasurement> memoryMeasurement,
			IEnumerable<Bot.MotionRecommendation> sequenceMotion)
		{
			var processId = memoryMeasurement?.ProcessId;

			if (!processId.HasValue || null == sequenceMotion)
				return;

			var process = System.Diagnostics.Process.GetProcessById(processId.Value);

			if (null == process)
				return;

			var motor = new WindowMotor(process.MainWindowHandle);

			var listMotionResult = new List<Bot.MotionResult>();

			foreach (var motion in sequenceMotion.EmptyIfNull())
			{
				var motionResult = motor.ActSequenceMotion(motion.MotionParam.AsSequenceMotion(memoryMeasurement?.Value));

				listMotionResult.Add(new Bot.MotionResult
				{
					Id = motion.Id,
					Success = motionResult?.Success ?? false,
				});
			}

			BotStepLastMotionResult = listMotionResult.ToArray();

			Thread.Sleep(FromMotionToMeasurementDelayMilli);
		}
	}
}
