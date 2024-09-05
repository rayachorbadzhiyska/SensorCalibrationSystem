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
            serialPort.NewLine = "\n";
            serialPort.StopBits = StopBits.One;
            serialPort.Parity = Parity.None;
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
        public void Connect(string port, string baudRate)
        {
            serialPort.PortName = port;
            serialPort.BaudRate = String.IsNullOrEmpty(baudRate) ? 115200 : int.Parse(baudRate);

            serialPort.Open();
            serialPort.DataReceived += BoardCommunicationService_SerialDataReceived;
        }

        /// <inheritdoc />
        public void Disconnect()
        {
            if (serialPort.IsOpen)
            {
                serialPort.DataReceived -= BoardCommunicationService_SerialDataReceived;
                serialPort.Close();
            }
        }

        /// <inheritdoc />
        public void Terminate()
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
        public string ReadLine()
        {
            if (serialPort.IsOpen)
            {
                return serialPort.ReadLine();
            }

            return string.Empty;
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
