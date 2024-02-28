using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SensorCalibrationSystem.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;

namespace SensorCalibrationSystem.ViewModels
{
    public class SerialPortConfigurationViewModel : ObservableObject
    {
        private readonly IBoardCommunicationService boardCommunicationService;
        private string selectedSerialPort;
        private string selectedDevice;

        public string SelectedSerialPort
        {
            get => selectedSerialPort;
            set => SetProperty(ref selectedSerialPort, value);
        }

        public string SelectedDevice
        {
            get => selectedDevice;
            set => SetProperty(ref selectedDevice, value);
        }

        public int MaxBaudRate { get; set; } = int.MaxValue;

        public List<string> SerialPorts { get; }

        public IRelayCommand ConfigureSerialPortCommand { get; }

        public event EventHandler OnSuccessfullyConfigured;

        public SerialPortConfigurationViewModel(IBoardCommunicationService boardCommunicationService)
        {
            this.boardCommunicationService = boardCommunicationService;

            SerialPorts = GetPortNames() ?? new List<string>();
            SelectedSerialPort = SerialPorts.FirstOrDefault();

            ConfigureSerialPortCommand = new RelayCommand(ConfigureSerialPort);
        }

        private List<string> GetPortNames()
        {
            List<string> ports = new List<string>();

            try
            {
                using (var searcher = new ManagementObjectSearcher(@"SELECT * FROM Win32_SerialPort"))
                {
                    foreach (ManagementObject queryObj in searcher.Get())
                    {
                        var test = queryObj.Properties;

                        string deviceID = queryObj["DeviceID"].ToString();
                        ports.Add(deviceID);

                        if (int.TryParse(queryObj["MaxBaudRate"].ToString(), out int maxBaudRate))
                        {
                            MaxBaudRate = maxBaudRate;
                        }
                    }
                }
            }
            catch (ManagementException e)
            {
                MessageBox.Show(e.Message, "An error has occurred.", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return ports;
        }

        private List<int> GetBaudRates()
        {
            var options = new JsonSerializerOptions()
            {
                NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString
            };

            string filePath = @"Resources\baud-rates.json";
            string jsonData = File.ReadAllText(filePath);

            List<int> allBaudRates = JsonSerializer.Deserialize<List<int>>(jsonData, options);
            List<int> availableBaudRates = allBaudRates.Where(x => x <= MaxBaudRate).ToList();

            return availableBaudRates;
        }

        private void ConfigureSerialPort()
        {
            try
            {
                boardCommunicationService.Connect(selectedSerialPort);

                OnSuccessfullyConfigured?.Invoke(this, null);
            }
            catch
            {
                MessageBox.Show($"Couldn't connect on port {selectedSerialPort}!\nCheck if the port exists or is already in use.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
