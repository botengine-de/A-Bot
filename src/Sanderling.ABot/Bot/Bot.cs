using Bib3;
using Sanderling.Parse;
using BotEngine.Interface;
using System.Linq;
using System.Collections.Generic;
using System;
using Sanderling.Motor;
using BotEngine.Motor;

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

				var sequenceTask =
					((IBotTask)new BotTask { Component = SequenceRootTask() })?.EnumerateNodeFromTreeDFirst(node => node?.Component);

				var sequenceTaskLeaf = sequenceTask?.Where(task => null != task.Motion);

				var taskNext = sequenceTaskLeaf?.FirstOrDefault();

				var motion = taskNext?.Motion;

				if (null != motion)
					addMotion(motion);
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

		IEnumerable<IBotTask> SequenceRootTask()
		{
			var setModuleShouldBeTurnedOn =
				MemoryMeasurementAccu?.ShipUiModule?.Where(module => module?.TooltipLast?.Value?.IsHardener ?? false);

			var moduleTurnOn =
				setModuleShouldBeTurnedOn?.FirstOrDefault(module => !(module?.RampActive ?? false));

			yield return new BotTask { Motion = moduleTurnOn?.MouseClick(MouseButtonIdEnum.Left) };

			var moduleUnknown = MemoryMeasurementAccu?.ShipUiModule?.FirstOrDefault(module => null == module?.TooltipLast?.Value);

			yield return new BotTask { Motion = moduleUnknown?.MouseMove() };
		}
	}
}
