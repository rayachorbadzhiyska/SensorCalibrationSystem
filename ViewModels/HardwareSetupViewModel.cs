using SensorCalibrationSystem.Contracts;
using System;

namespace SensorCalibrationSystem.ViewModels
{
    public class HardwareSetupViewModel : INavigationPage
    {
        #region Properties Interfaces

        public string Name { get; set; } = "Hardware Setup";

        public string Logo { get; set; }

        public int NavigationIndex { get; set; }

        public bool IsActive { get; set; }

        #endregion

        public PrintedCircuitBoard3DViewModel PrintedCircuitBoard3DViewModel { get; }

        public HardwareSetupViewModel(PrintedCircuitBoard3DViewModel printedCircuitBoard3DViewModel)
        {
            PrintedCircuitBoard3DViewModel = printedCircuitBoard3DViewModel ?? throw new ArgumentNullException(nameof(printedCircuitBoard3DViewModel));
        }
    }
}
