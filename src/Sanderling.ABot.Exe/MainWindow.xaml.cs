using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Sanderling.ABot.Exe
{
	public partial class MainWindow : Window
	{
		public string TitleComputed =>
			"A-Bot v" + (TryFindResource("AppVersionId") ?? "");

		public MainWindow()
		{
			InitializeComponent();
		}

		private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			ProcessInput();
		}

		public void ProcessInput()
		{
			if (App.SetKeyBotMotionDisable?.Any(setKey => setKey?.All(key => Keyboard.IsKeyDown(key)) ?? false) ?? false)
				Main?.BotMotionDisable();
		}
	}
}
