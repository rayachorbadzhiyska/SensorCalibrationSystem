using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SensorCalibrationSystem.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Windows;

namespace SensorCalibrationSystem.ViewModels
{
    public class SerialPortConfigurationViewModel : ObservableObject
    {
        private readonly IBoardCommunicationService boardCommunicationService;
        private readonly string baudRatesFilePath = @"Resources\baud-rates.json";

        private string? selectedSerialPort;
        private string? selectedDevice;
        private List<string> baudRates;
        private string selectedBaudRate;

        public string? SelectedSerialPort
        {
            get => selectedSerialPort;
            set => SetProperty(ref selectedSerialPort, value);
        }

        public string? SelectedDevice
        {
            get => selectedDevice;
            set => SetProperty(ref selectedDevice, value);
        }

        public List<string> SerialPorts { get; }

        public List<string> BaudRates
        {
            get => baudRates;
            set => SetProperty(ref baudRates, value);
        }

        public string SelectedBaudRate
        {
            get => selectedBaudRate;
            set => SetProperty(ref selectedBaudRate, value);
        }

        public IRelayCommand ConfigureSerialPortCommand { get; }

        public IRelayCommand BaudRateChangedCommand { get; }

        public event EventHandler? OnSuccessfullyConfigured;

        public SerialPortConfigurationViewModel(IBoardCommunicationService boardCommunicationService)
        {
            this.boardCommunicationService = boardCommunicationService;

            SerialPorts = GetPortNames() ?? new List<string>();
            SelectedSerialPort = SerialPorts.FirstOrDefault();

            ConfigureSerialPortCommand = new RelayCommand(ConfigureSerialPort);
            BaudRateChangedCommand = new RelayCommand(OnBaudRateChanged);
        }

        private List<string> GetPortNames() => SerialPort.GetPortNames().ToList();

        private void ConfigureSerialPort()
        {
            try
            {
                if (selectedSerialPort is not null)
                {
                    boardCommunicationService.Connect(selectedSerialPort);
                }

                OnSuccessfullyConfigured?.Invoke(this, new EventArgs());
            }
            catch
            {
                MessageBox.Show($"Couldn't connect on port {selectedSerialPort}!\nCheck if the port exists or is already in use.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void OnBaudRateChanged()
        {
            boardCommunicationService.Disconnect();
            boardCommunicationService.Connect(SelectedSerialPort, SelectedBaudRate);
        }

        public void LoadBaudRates()
        {
            string directoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string fullBaudRatesFilePath = Path.Combine(directoryPath, baudRatesFilePath);

            if (File.Exists(fullBaudRatesFilePath))
            {
                string? jsonData = File.ReadAllText(baudRatesFilePath);

                if (string.IsNullOrEmpty(jsonData))
                {
                    MessageBox.Show("Baud rates file is empty", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                List<string> baudRatesList = JsonSerializer.Deserialize<List<string>>(jsonData);

                if (baudRatesList is not null && baudRatesList.Any())
                {
                    BaudRates = baudRatesList;
                    SelectedBaudRate = BaudRates.FirstOrDefault(x => x == "115200");
                }
            }
            else
            {
                MessageBox.Show("Baud rates file does not exist", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
