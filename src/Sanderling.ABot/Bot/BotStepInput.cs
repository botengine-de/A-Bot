﻿using System;

namespace Sanderling.ABot.Bot
{
	public class MotionResult
	{
		public Int64 Id;

		public bool Success;
	}

	public class BotStepInput
	{
		public Int64 TimeMilli;

		public BotEngine.Interface.FromProcessMeasurement<Interface.MemoryStruct.IMemoryMeasurement> FromProcessMemoryMeasurement;

		public string PreferencesSerial;

		public MotionResult[] StepLastMotionResult;
	}
}
