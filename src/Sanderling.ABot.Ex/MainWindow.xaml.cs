using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Sanderling.ABot.Exe
{
    public partial class MainWindow : Window
    {
        static public event Action<Interface.MemoryStruct.IMemoryMeasurement> SimulateMeasurement;

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

        private void TextBlock_Drop(object sender, DragEventArgs e)
        {
            Bib3.FCL.GBS.Extension.CatchNaacMessageBoxException(() =>
            {
                var file = Bib3.FCL.Glob.LaadeFrüühestInhaltDataiAusDropFileDrop(e);

                try
                {
                    var memoryMeasurementJson = Encoding.UTF8.GetString(file.Value);

                    var graph = Bib3.RefNezDiferenz.Extension.ListeWurzelDeserialisiireVonJson(memoryMeasurementJson)?.FirstOrDefault();

                    if (null == graph)
                        throw new Exception("failed to read graph.");

                    var measurement =
                        (graph as BotEngine.Interface.FromProcessMeasurement<Interface.MemoryStruct.IMemoryMeasurement>)?.Value ??
                        graph as Interface.MemoryStruct.IMemoryMeasurement;

                    if (null == measurement)
                        throw new ArgumentException("unexpected type:" + graph?.GetType());

                    SimulateMeasurement?.Invoke(measurement);
                }
                catch (Exception exc)
                {
                    throw new Exception("Loaded from file " + file.Key, exc);
                }
            });
        }
    }
}
