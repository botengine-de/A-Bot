using Bib3.Synchronization;
using BotEngine.Interface;
using Sanderling.Interface.MemoryStruct;
using System;
using System.Threading.Tasks;

namespace Sanderling.ABot.Exe
{
	/// <summary>
	/// This Type must reside in an Assembly that can be resolved by the default assembly resolver.
	/// </summary>
	public class InterfaceAppDomainSetup
	{
		static InterfaceAppDomainSetup()
		{
			BotEngine.Interface.InterfaceAppDomainSetup.Setup();
		}
	}

	partial class App
	{
		FromProcessMeasurement<IMemoryMeasurement> MemoryMeasurementLast;

		readonly SimpleInterfaceServerDispatcher SensorServerDispatcher = new SimpleInterfaceServerDispatcher
		{
			InterfaceAppDomainSetupType = typeof(InterfaceAppDomainSetup),
			InterfaceAppDomainSetupTypeLoadFromMainModule = true,
			LicenseClientConfig = Sanderling.ExeConfig.LicenseClientDefault,
		};

		readonly Bib3.RateLimit.IRateLimitStateInt MemoryMeasurementRequestRateLimit = new Bib3.RateLimit.RateLimitStateIntSingle();

		void InterfaceExchange()
		{
			var eveOnlineClientProcessId = EveOnlineClientProcessId;

			var measurementRequestTime = MeasurementRequestTime() ?? 0;

			if (eveOnlineClientProcessId.HasValue && measurementRequestTime <= GetTimeStopwatch())
				if (MemoryMeasurementRequestRateLimit.AttemptPass(GetTimeStopwatch(), 700))
					Task.Run(() => botLock.IfLockIsAvailableEnter(() => MeasurementMemoryTake(eveOnlineClientProcessId.Value, measurementRequestTime)));
		}

		void MeasurementMemoryTake(int processId, Int64 measurementBeginTimeMinMilli)
		{
			var measurement = SensorServerDispatcher.InterfaceAppManager.MeasurementTake(processId, measurementBeginTimeMinMilli);

			if (null == measurement)
				return;

			MemoryMeasurementLast = measurement;
		}
	}
}
