using SensorCalibrationSystem.Contracts;

namespace SensorCalibrationSystem.ViewModels
{
    public class CalibrationViewModel : INavigationPage
    {
        #region Properties Interfaces

        public string Name { get; set; } = "Calibration";

        public string Logo { get; set; } = "/Resources/calibration.png";

        public int NavigationIndex { get; set; }

        public bool IsActive { get; set; }

        #endregion

        #region Navigation Methods

        public void OnNavigatedTo()
        {
        }

        public void OnNavigatedFrom()
        {
        }

        #endregion
    }
}
