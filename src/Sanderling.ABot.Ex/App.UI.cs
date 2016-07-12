using Sanderling.ABot.UI;

namespace Sanderling.ABot.Exe
{
	partial class App
	{
		MainWindow Window => MainWindow as MainWindow;

		Main MainControl => Window?.Main;

		Sanderling.UI.InterfaceToEve InterfaceToEveControl => Window?.Main?.Interface;

		public int? EveOnlineClientProcessId => InterfaceToEveControl?.ProcessChoice?.ChoosenProcessId;

		void UIPresent()
		{
			InterfaceToEveControl?.Present(SensorServerDispatcher, MemoryMeasurementLast);
		}
	}
}
