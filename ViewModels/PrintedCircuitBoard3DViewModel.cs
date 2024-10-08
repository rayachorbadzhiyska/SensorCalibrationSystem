using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SensorCalibrationSystem.Contracts;
using SensorCalibrationSystem.Models;
using System;
using System.Globalization;

namespace SensorCalibrationSystem.ViewModels
{
    public class PrintedCircuitBoard3DViewModel : ObservableObject
    {
        #region Fields

        private readonly IBoardCommunicationService boardCommunicationService;

        private QuaternionModel? quaternionModel;

        #endregion

        #region Properties

        public QuaternionModel? QuaternionModel
        {
            get => quaternionModel;
            set => SetProperty(ref quaternionModel, value);
        }

        #endregion

        #region Event Handlers

        public event EventHandler<QuaternionModel>? QuaternionValuesReceived;

        #endregion

        #region Constructor

        public PrintedCircuitBoard3DViewModel(IBoardCommunicationService boardCommunicationService)
        {
            this.boardCommunicationService = boardCommunicationService ?? throw new ArgumentNullException(nameof(boardCommunicationService));
        }

        #endregion

        #region Event Handlers

        private void SerialPort_DataReceived(object? sender, string data)
        {
            try
            {
                // Process the received data (e.g., extract quaternion values)
                string[] values = data.Split(' ');
                if (values.Length == 5 && values[0] == "Quaternion:")
                {
                    float x = float.Parse(values[1], CultureInfo.InvariantCulture.NumberFormat);
                    float y = float.Parse(values[2], CultureInfo.InvariantCulture.NumberFormat);
                    float z = float.Parse(values[3], CultureInfo.InvariantCulture.NumberFormat);
                    float w = float.Parse(values[4], CultureInfo.InvariantCulture.NumberFormat);

                    QuaternionModel quaternion = new QuaternionModel(x, y, z, w);

                    QuaternionModel = quaternion;

                    QuaternionValuesReceived?.Invoke(this, quaternion);
                }
            }
            catch
            {
                // Since Serial port reading threw an error, then there is no value to be parsed
                // hence exit the function.
                return;
            }
        }

        private void BoardCommunicationService_ConnectionChanged(object? sender, bool hasConnection)
        {
            if (hasConnection)
            {
                boardCommunicationService.WriteLine("START_QUATERNION_VALUES_STREAMING");
            }
            else
            {
                boardCommunicationService.WriteLine("STOP_QUATERNION_VALUES_STREAMING");
            }
        }

        #endregion

        #region Methods

        public void Load()
        {
            boardCommunicationService.SerialDataReceived += SerialPort_DataReceived;
            boardCommunicationService.ConnectionChanged += BoardCommunicationService_ConnectionChanged;

            boardCommunicationService.WriteLine("START_QUATERNION_VALUES_STREAMING");
        }

        public void Unload()
        {
            boardCommunicationService.ConnectionChanged -= BoardCommunicationService_ConnectionChanged;
            boardCommunicationService.SerialDataReceived -= SerialPort_DataReceived;

            boardCommunicationService.WriteLine("STOP_QUATERNION_VALUES_STREAMING");
        }

        #endregion
    }
}
