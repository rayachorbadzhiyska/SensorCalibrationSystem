using System;

namespace SensorCalibrationSystem.Contracts
{
    /// <summary>
    /// Represents a service, used to communicate with the attached board.
    /// </summary>
    public interface IBoardCommunicationService
    {
        /// <summary>
        /// An event, which should be handled whenever serial data is received.
        /// </summary>
        event EventHandler<string>? SerialDataReceived;

        /// <summary>
        /// Connects to the board on the specified serial port.
        /// </summary>
        /// <param name="port">The serial port.</param>
        void Connect(string port);

        /// <summary>
        /// Disconnects from the board.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Writes a line with the message.
        /// </summary>
        /// <param name="message">The message to be sent out.</param>
        void WriteLine(string message);
    }
}
