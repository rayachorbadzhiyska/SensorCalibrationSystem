using System;

namespace SensorCalibrationSystem.ViewModels
{
    public class MainViewModel
    {
        public PrintedCircuitBoard3DViewModel PrintedCircuitBoard3DViewModel { get; }

        public MainViewModel(PrintedCircuitBoard3DViewModel printedCircuitBoard3DViewModel)
        {
            PrintedCircuitBoard3DViewModel = printedCircuitBoard3DViewModel ?? throw new ArgumentNullException(nameof(printedCircuitBoard3DViewModel));
        }
    }
}
