namespace SensorCalibrationSystem.Contracts
{
    /// <summary>
    /// Represents a reloadable object.
    /// </summary>
    public interface IReloadable
    {
        /// <summary>
        /// Gets or sets whether the object has been loaded.
        /// </summary>
        public bool HasBeenLoaded { get; set; }
    }
}
