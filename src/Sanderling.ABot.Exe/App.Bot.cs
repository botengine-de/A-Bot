using Bib3;
using Bib3.Synchronization;
using BotEngine.Interface;
using Sanderling.Interface.MemoryStruct;
using Sanderling.Motor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Sanderling.ABot.Exe
{
	partial class App
	{
		readonly object botLock = new object();

		readonly Bot.Bot bot = new Bot.Bot();

		const int FromMotionToMeasurementDelayMilli = 300;

		const int MemoryMeasurementDistanceMaxMilli = 3000;

		const string BotConfigFileName = "bot.config";

		PropertyGenTimespanInt64<Bot.MotionResult[]> BotStepLastMotionResult;

		PropertyGenTimespanInt64<KeyValuePair<Exception, StringAtPath>> BotConfigLoaded;

		Int64? MeasurementRequestTime()
		{
			var memoryMeasurementLast = MemoryMeasurementLast;

			var botStepLastMotionResult = BotStepLastMotionResult;

			if (memoryMeasurementLast?.Begin < botStepLastMotionResult?.End && 0 < botStepLastMotionResult?.Value?.Length)
				return botStepLastMotionResult?.End + FromMotionToMeasurementDelayMilli;

			return memoryMeasurementLast?.Begin + MemoryMeasurementDistanceMaxMilli;
		}

		void BotProgress(bool motionEnable)
		{
			botLock.IfLockIsAvailableEnter(() =>
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
					StepLastMotionResult = BotStepLastMotionResult?.Value,
					ConfigSerial = BotConfigLoaded?.Value.Value,
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

			var startTime = GetTimeStopwatch();

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

			BotStepLastMotionResult = new PropertyGenTimespanInt64<Bot.MotionResult[]>(listMotionResult.ToArray(), startTime, GetTimeStopwatch());

			Thread.Sleep(FromMotionToMeasurementDelayMilli);
		}

		void BotConfigLoad()
		{
			Exception exception = null;
			string configString = null;
			var configFilePath = AssemblyDirectoryPath.PathToFilesysChild(BotConfigFileName);

			try
			{

				using (var fileStream = new FileStream(configFilePath, FileMode.Open, FileAccess.Read))
					configString = new StreamReader(fileStream).ReadToEnd();
			}
			catch (Exception e)
			{
				exception = e;
			}

			BotConfigLoaded = new PropertyGenTimespanInt64<KeyValuePair<Exception, StringAtPath>>(new KeyValuePair<Exception, StringAtPath>(
				exception,
				new StringAtPath { Path = configFilePath, String = configString }), GetTimeStopwatch());
		}
	}
}
