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

		int stepIndex;

		readonly Accumulator.MemoryMeasurementAccumulator MemoryMeasurementAccu = new Accumulator.MemoryMeasurementAccumulator();

		IDictionary<Int64, int> MouseClickLastStepIndexFromUIElementId = new Dictionary<Int64, int>();

		Int64? MouseClickLastAgeStepCountFromUIElement(Interface.MemoryStruct.IUIElement uiElement)
		{
			if (null == uiElement)
				return null;

			var interactionLastStepIndex = MouseClickLastStepIndexFromUIElementId?.TryGetValueNullable(uiElement.Id);

			return stepIndex - interactionLastStepIndex;
		}

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
				var setMotionMOuseWaypointUIElement =
					listMotion
					?.Select(motion => motion?.MotionParam)
					?.Where(motionParam => 0 < motionParam?.MouseButton?.Count())
					?.Select(motionParam => motionParam?.MouseListWaypoint)
					?.ConcatNullable()?.Select(mouseWaypoint => mouseWaypoint?.UIElement)?.WhereNotDefault();

				foreach (var mouseWaypointUIElement in setMotionMOuseWaypointUIElement.EmptyIfNull())
					MouseClickLastStepIndexFromUIElementId[mouseWaypointUIElement.Id] = stepIndex;

				stepLastResult = stepResult = new BotStepResult
				{
					ListMotion = listMotion?.ToArrayIfNotEmpty(),
				};

				++stepIndex;
			}

			return stepResult;
		}

		IEnumerable<IBotTask> SequenceRootTask()
		{
			var setModuleShouldBeTurnedOn =
				MemoryMeasurementAccu?.ShipUiModule?.Where(module => module?.TooltipLast?.Value?.IsHardener ?? false);

			var moduleTurnOn =
				setModuleShouldBeTurnedOn?.FirstOrDefault(module => !(module?.RampActive ?? false) && !(MouseClickLastAgeStepCountFromUIElement(module) <= 1));

			yield return new BotTask { Motion = moduleTurnOn?.MouseClick(MouseButtonIdEnum.Left) };

			var moduleUnknown = MemoryMeasurementAccu?.ShipUiModule?.FirstOrDefault(module => null == module?.TooltipLast?.Value);

			yield return new BotTask { Motion = moduleUnknown?.MouseMove() };
		}
	}
}
