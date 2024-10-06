using SensorCalibrationSystem.Contracts;
using System;
using System.IO.Ports;
using System.Linq;
using System.Management;

namespace SensorCalibrationSystem.Services
{
    /// <inheritdoc />
    public class BoardCommunicationService : IBoardCommunicationService
    {
        private SerialPort serialPort;
        private bool isConnected;

        /// <inheritdoc />
        public event EventHandler<string>? SerialDataReceived;

        public event EventHandler<bool>? ConnectionChanged;

        public BoardCommunicationService()
        {
            serialPort = new();
            serialPort.NewLine = "\n";
            serialPort.StopBits = StopBits.One;
            serialPort.Parity = Parity.None;

            WatchForUSBEvents();
        }

        private void WatchForUSBEvents()
        {
            // Watch for USB connection (EventType = 2) and disconnection (EventType = 3)
            WqlEventQuery insertQuery = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 2"); // Insertion
            WqlEventQuery removeQuery = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 3"); // Removal

            ManagementEventWatcher insertWatcher = new ManagementEventWatcher(insertQuery);
            insertWatcher.EventArrived += new EventArrivedEventHandler(DeviceInsertedEvent);
            insertWatcher.Start();

            ManagementEventWatcher removeWatcher = new ManagementEventWatcher(removeQuery);
            removeWatcher.EventArrived += new EventArrivedEventHandler(DeviceRemovedEvent);
            removeWatcher.Start();
        }

        private void DeviceInsertedEvent(object sender, EventArrivedEventArgs e)
        {
            var availablePorts = SerialPort.GetPortNames().ToList();

            if (availablePorts.Contains(serialPort.PortName) && !isConnected)
            {
                Connect(serialPort.PortName, serialPort.BaudRate.ToString());
            }
        }

        private void DeviceRemovedEvent(object sender, EventArrivedEventArgs e)
        {
            if (isConnected)
            {
                Disconnect();
            }
        }

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

            serialPort.DataReceived += BoardCommunicationService_SerialDataReceived;

            System.Threading.Thread.Sleep(2000);

            serialPort.Open();

            ConnectionChanged?.Invoke(this, true);

            isConnected = true;
        }

        /// <inheritdoc />
        public void Disconnect()
        {
            serialPort.DataReceived -= BoardCommunicationService_SerialDataReceived;

            serialPort.Close();
            System.Threading.Thread.Sleep(2000);

            ConnectionChanged?.Invoke(this, false);

            isConnected = false;
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

            ConnectionChanged?.Invoke(this, false);

            isConnected = false;
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

        /// <inheritdoc />
        public bool CheckConnection()
        {
            return serialPort.IsOpen;
        }
    }
}
