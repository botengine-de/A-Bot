using System;
using System.Windows;
using System.Windows.Threading;

namespace Sanderling.ABot.Exe
{
	public partial class App : Application
	{
		static public Int64 GetTimeStopwatch() => Bib3.Glob.StopwatchZaitMiliSictInt();

		public App()
		{
			SensorServerDispatcher.CyclicExchangeStart();

			TimerConstruct();
		}

		void TimerConstruct()
		{
			var	timer = new DispatcherTimer(TimeSpan.FromSeconds(1.0 / 10), DispatcherPriority.Normal, Timer_Tick, Dispatcher);

			timer.Start();
		}

		void Timer_Tick(object sender, object e)
		{
			InterfaceExchange();

			UIPresent();
		}
	}
}
