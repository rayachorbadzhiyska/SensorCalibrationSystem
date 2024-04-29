using SensorCalibrationSystem.Contracts;

namespace SensorCalibrationSystem.ViewModels
{
    public class MemoryMapViewModel : INavigationPage
    {
        #region Properties Interfaces

        public string Name { get; set; } = "Memory Map";

        public string Logo { get; set; } = "/Resources/memory-map.png";

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
