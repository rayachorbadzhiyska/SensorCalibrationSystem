using CommunityToolkit.Mvvm.Input;
using SensorCalibrationSystem.Contracts;
using SensorCalibrationSystem.Models;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;

namespace SensorCalibrationSystem.ViewModels
{
    public class HardwareSetupViewModel : INavigationPage, IReloadable
    {
        #region Properties Interfaces

        public string Name { get; set; } = "Hardware Setup";

        public string Logo { get; set; }

        public int NavigationIndex { get; set; }

        public bool IsActive { get; set; }

        public bool HasBeenLoaded { get; set; }

        #endregion

        #region Fields

        private readonly string sensorTechnicalDataPath = @"sensors-technical-data";

        #endregion

        #region Properties

        public ObservableCollection<SensorModel> Sensors { get; set; }

        public PrintedCircuitBoard3DViewModel PrintedCircuitBoard3DViewModel { get; }

        #endregion

        #region Commands

        public IRelayCommand OnLoadedCommand { get; }

        #endregion

        #region Constructor

        public HardwareSetupViewModel(PrintedCircuitBoard3DViewModel printedCircuitBoard3DViewModel)
        {
            PrintedCircuitBoard3DViewModel = printedCircuitBoard3DViewModel ?? throw new ArgumentNullException(nameof(printedCircuitBoard3DViewModel));

            Sensors = new();

            OnLoadedCommand = new RelayCommand(OnLoaded);
        }

        #endregion

        #region Methods

        private void LoadSensorsTechnicalData()
        {
            string[] files = Directory.GetFiles(sensorTechnicalDataPath);

            foreach (var file in files)
            {
                string jsonData = File.ReadAllText(file);

                SensorModel sensor = JsonSerializer.Deserialize<SensorModel>(jsonData);

                Sensors.Add(sensor);
            }
        }

        private void OnLoaded()
        {
            LoadSensorsTechnicalData();
        }

        #endregion
    }
}
