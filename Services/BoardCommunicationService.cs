using SensorCalibrationSystem.Contracts;
using System;
using System.IO.Ports;
using System.Linq;

namespace SensorCalibrationSystem.Services
{
    /// <inheritdoc />
    public class BoardCommunicationService : IBoardCommunicationService
    {
        private SerialPort serialPort;

        /// <inheritdoc />
        public event EventHandler<string>? SerialDataReceived;

        public BoardCommunicationService()
        {
            serialPort = new();
            serialPort.BaudRate = 115200;
            serialPort.NewLine = "\n";
            serialPort.StopBits = StopBits.One;
            serialPort.Parity = Parity.None;
            serialPort.DataReceived += BoardCommunicationService_SerialDataReceived;
        }

        /// <inheritdoc />
        private void BoardCommunicationService_SerialDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string? serialData = serialPort?.ReadLine();

            if (!string.IsNullOrEmpty(serialData))
            {
                SerialDataReceived?.Invoke(this, serialData);
            }
        }

        /// <inheritdoc />
        public void Connect(string port)
        {
            serialPort.PortName = port;

            serialPort.Open();
        }

        /// <inheritdoc />
        public void Disconnect()
        {
            if (serialPort.IsOpen)
            {
                serialPort.DataReceived -= BoardCommunicationService_SerialDataReceived;
                serialPort.BaseStream.Close();
                serialPort.Close();
                serialPort.Dispose();
            }
        }

        /// <inheritdoc />
        public void WriteLine(string message)
        {
            if (serialPort.IsOpen)
            {
                serialPort.WriteLine(message);
            }
        }
    }
}
