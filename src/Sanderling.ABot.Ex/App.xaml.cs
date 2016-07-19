using Bib3;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Sanderling.ABot.Exe
{
	public partial class App : Application
	{
		static string AssemblyDirectoryPath => Bib3.FCL.Glob.ZuProcessSelbsctMainModuleDirectoryPfaadBerecne().EnsureEndsWith(@"\");

		static public Int64 GetTimeStopwatch() => Bib3.Glob.StopwatchZaitMiliSictInt();

		public App()
		{
			BotConfigLoad();

			AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

			SensorServerDispatcher.CyclicExchangeStart();

			TimerConstruct();
		}

		private System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			var matchFullName =
				AppDomain.CurrentDomain.GetAssemblies()
				?.FirstOrDefault(candidate => string.Equals(candidate.GetName().FullName, args?.Name));

			if (null != matchFullName)
				return matchFullName;

			var matchName =
				AppDomain.CurrentDomain.GetAssemblies()
				?.FirstOrDefault(candidate => string.Equals(candidate.GetName().Name, args?.Name));

			return matchName;
		}

		void TimerConstruct()
		{
			var timer = new DispatcherTimer(TimeSpan.FromSeconds(1.0 / 10), DispatcherPriority.Normal, Timer_Tick, Dispatcher);

			timer.Start();
		}

		void Timer_Tick(object sender, object e)
		{
			Window?.ProcessInput();

			InterfaceExchange();

			UIPresent();

			var motionEnable = MainControl?.IsBotMotionEnabled ?? false;

			Task.Run(() => BotProgress(motionEnable));
		}

		private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			try
			{
				var filePath = AssemblyDirectoryPath.PathToFilesysChild(DateTime.Now.SictwaiseKalenderString(".", 0) + " Exception");

				filePath.WriteToFileAndCreateDirectoryIfNotExisting(Encoding.UTF8.GetBytes(e.Exception.SictString()));

				var message = "exception written to file: " + filePath;

				MessageBox.Show(message, message, MessageBoxButton.OK, MessageBoxImage.Exclamation);
			}
			catch (Exception PersistException)
			{
				Bib3.FCL.GBS.Extension.MessageBoxException(PersistException);
			}

			Bib3.FCL.GBS.Extension.MessageBoxException(e.Exception);

			e.Handled = true;
		}
	}
}
