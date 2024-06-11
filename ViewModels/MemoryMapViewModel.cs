using SensorCalibrationSystem.Contracts;
using SensorCalibrationSystem.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;

namespace SensorCalibrationSystem.ViewModels
{
    public class MemoryMapViewModel : INavigationPage, IReloadable
    {
        #region Properties Interfaces

        public string Name { get; set; } = "Memory Map";

        public string Logo { get; set; } = "/Resources/memory-map.png";

        public int NavigationIndex { get; set; }

        public bool IsActive { get; set; }

        public bool HasBeenLoaded { get; set; }

        #endregion

        #region Fields

        private readonly string sensorMemoryMapDataPath = @"memory-maps";

        #endregion

        #region Properties

        public ObservableCollection<MemoryMapModel> MemoryMaps { get; set; }

        #endregion

        #region Constructor

        public MemoryMapViewModel()
        {
            MemoryMaps = new ObservableCollection<MemoryMapModel>();
        }

        #endregion

        #region Methods

        private void LoadMemoryMapData()
        {
            string[] files = Directory.GetFiles(sensorMemoryMapDataPath);

            foreach (var file in files)
            {
                string jsonData = File.ReadAllText(file);

                MemoryMapModel memoryMap = JsonSerializer.Deserialize<MemoryMapModel>(jsonData);

                if (memoryMap is not null)
                {
                    MemoryMaps.Add(memoryMap);
                }
            }
        }

        #endregion

        #region Navigation Methods

        public void OnNavigatedTo()
        {
            if (!HasBeenLoaded)
            {
                LoadMemoryMapData();

                HasBeenLoaded = true;
            }
        }

        public void OnNavigatedFrom()
        {
        }

        #endregion
    }
}
