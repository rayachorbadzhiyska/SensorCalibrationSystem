using CefSharp;
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
    public partial class PrintedCircuitBoard3DView : UserControl
    {
        PrintedCircuitBoard3DViewModel viewModel;

        public PrintedCircuitBoard3DView()
        {
            InitializeComponent();
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
                    viewModel.QuaternionValuesReceived += PCB3DView_QuaternionValuesReceived;
                }
            };
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
            this.viewModel.QuaternionValuesReceived -= PCB3DView_QuaternionValuesReceived;
        }
    }
}
