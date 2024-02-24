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
        /// Connects to the board.
        /// </summary>
        void Connect();

        /// <summary>
        /// Disconnects from the board.
        /// </summary>
        void Disconnect();
    }
}
