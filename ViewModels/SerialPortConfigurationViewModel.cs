using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SensorCalibrationSystem.Contracts;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
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

        private List<string> GetPortNames() => SerialPort.GetPortNames().ToList();

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
