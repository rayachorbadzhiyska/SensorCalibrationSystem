using CefSharp;
using SensorCalibrationSystem.Contracts;
using SensorCalibrationSystem.Models;
using SensorCalibrationSystem.ViewModels;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace SensorCalibrationSystem.Views
{
    /// <summary>
    /// Interaction logic for PrintedCircuitBoard3DView.xaml
    /// </summary>
    public partial class PrintedCircuitBoard3DView : UserControl, IReloadable
    {
        private readonly string localServerPort = SensorCalibrationSystem.Resources.Resources.LocalServerPort;
        private PrintedCircuitBoard3DViewModel? viewModel;

        public bool HasBeenLoaded { get; set; }

        public PrintedCircuitBoard3DView()
        {
            InitializeComponent();
            PCB3DView.Address = $"localhost:{localServerPort}/PrintedCircuitBoard3D.html";
        }

        private void PrintedCircuit3DControl_Loaded(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as PrintedCircuitBoard3DViewModel;

            if (viewModel is null)
            {
                return;
            }

            this.viewModel = viewModel;

            PCB3DView.LoadingStateChanged += (sender, args) =>
            {
                if (!args.IsLoading)
                {
                    // Create the box design only once.
                    if (!HasBeenLoaded)
                    {
                        createBoxDesign();

                        HasBeenLoaded = true;
                    }

                    viewModel.QuaternionValuesReceived += PCB3DView_QuaternionValuesReceived;
                }
            };
        }

        private void createBoxDesign()
        {
            string frontDesignImagePath = @$"'http://localhost:{localServerPort}/Resources/board-3d-front.png'";
            string backDesignImagePath = @$"'http://localhost:{localServerPort}/Resources/board-3d-back.png'";

            string script = $"createBoxDesign({frontDesignImagePath}, {backDesignImagePath});";

            PCB3DView.ExecuteScriptAsync(script);
        }

        private void PCB3DView_QuaternionValuesReceived(object? sender, QuaternionModel e)
        {
            var culture = CultureInfo.InvariantCulture;
            string script = $"updateSceneWithQuaternion({e.XValue.ToString(culture)}, " +
                $"{e.YValue.ToString(culture)}, " +
                $"{e.ZValue.ToString(culture)}, " +
                $"{e.WValue.ToString(culture)});";

            PCB3DView.ExecuteScriptAsync(script);
        }

        private void PrintedCircuit3DControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (viewModel is not null)
            {
                viewModel.QuaternionValuesReceived -= PCB3DView_QuaternionValuesReceived;
            }
        }
    }
}
