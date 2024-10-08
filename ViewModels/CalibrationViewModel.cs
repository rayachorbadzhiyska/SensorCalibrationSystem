using CommunityToolkit.Mvvm.ComponentModel;
using SensorCalibrationSystem.Contracts;
using SensorCalibrationSystem.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace SensorCalibrationSystem.ViewModels
{
    public class CalibrationViewModel : ObservableObject, INavigationPage, IReloadable
    {
        #region Fields

        private readonly IBoardCommunicationService boardCommunicationService;

        private readonly string sensorCalibrationDataPath = @"sensors-reference";

        private SensorCalibrationModel? selectedSensor;

        #endregion

        #region Properties Interfaces

        public string Name { get; set; } = "Calibration";

        public string Logo { get; set; } = "/Resources/calibration.png";

        public int NavigationIndex { get; set; }

        public bool IsActive { get; set; }

        public bool HasBeenLoaded { get; set; }

        #endregion

        #region Properties

        public ObservableCollection<SensorCalibrationModel> Sensors { get; set; }

        public SensorCalibrationModel? SelectedSensor
        {
            get => selectedSensor;
            set => SetProperty(ref selectedSensor, value);
        }

        #endregion

        #region Constructor

        public CalibrationViewModel(IBoardCommunicationService boardCommunicationService)
        {
            this.boardCommunicationService = boardCommunicationService;

            Sensors = new ObservableCollection<SensorCalibrationModel>();
        }

        #endregion

        #region Navigation Methods

        private void LoadSensorCalibrationData()
        {
            string[] files = Directory.GetFiles(sensorCalibrationDataPath);

            foreach (var file in files)
            {
                string jsonData = File.ReadAllText(file);

                SensorCalibrationModel? sensor = JsonSerializer.Deserialize<SensorCalibrationModel>(jsonData);

                if (sensor is not null)
                {
                    Sensors.Add(sensor);
                }
            }
        }

        public void OnNavigatedTo()
        {
            if (!HasBeenLoaded)
            {
                LoadSensorCalibrationData();

                if (Sensors.Any())
                {
                    SelectedSensor = Sensors.First();
                }

                HasBeenLoaded = true;
            }
        }

        public void OnNavigatedFrom()
        {
        }

        #endregion
    }
}
