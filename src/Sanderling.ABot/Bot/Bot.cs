using Bib3;
using Sanderling.Parse;
using BotEngine.Interface;

namespace Sanderling.ABot.Bot
{
	public class Bot
	{
		public BotStepInput StepLastInput { private set; get; }

		readonly Accumulator.MemoryMeasurementAccumulator MemoryMeasurementAccu = new Accumulator.MemoryMeasurementAccumulator();

		public void Step(BotStepInput input)
		{
			StepLastInput = input;

			var memoryMeasurementAtTime = input?.FromProcessMemoryMeasurement?.MapValue(measurement => measurement?.Parse());

			var memoryMeasurement = memoryMeasurementAtTime?.Value;

			MemoryMeasurementAccu.Accumulate(memoryMeasurementAtTime);
		}
	}
}
