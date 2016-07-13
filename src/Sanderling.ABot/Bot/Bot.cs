using Bib3;
using Sanderling.Parse;
using BotEngine.Interface;
using System.Linq;
using System.Collections.Generic;
using System;
using Sanderling.Motor;

namespace Sanderling.ABot.Bot
{
	public class Bot
	{
		public BotStepInput StepLastInput { private set; get; }

		BotStepResult stepLastResult;

		int motionId;

		readonly Accumulator.MemoryMeasurementAccumulator MemoryMeasurementAccu = new Accumulator.MemoryMeasurementAccumulator();

		public BotStepResult Step(BotStepInput input)
		{
			StepLastInput = input;

			var listMotion = new List<MotionRecommendation>();

			BotStepResult stepResult = null;

			try
			{
				var addMotion = new Action<MotionParam>(motionParam => listMotion.Add(new MotionRecommendation
				{
					Id = ++motionId,
					MotionParam = motionParam,
				}));

				var memoryMeasurementAtTime = input?.FromProcessMemoryMeasurement?.MapValue(measurement => measurement?.Parse());

				var memoryMeasurement = memoryMeasurementAtTime?.Value;

				MemoryMeasurementAccu.Accumulate(memoryMeasurementAtTime);

				var moduleUnknown = MemoryMeasurementAccu?.ShipUiModule?.FirstOrDefault(module => null == module?.TooltipLast?.Value);

				if (null != moduleUnknown)
					addMotion(moduleUnknown.MouseMove());
			}
			finally
			{
				stepLastResult = stepResult = new BotStepResult
				{
					ListMotion = listMotion?.ToArrayIfNotEmpty(),
				};
			}

			return stepResult;
		}
	}
}
