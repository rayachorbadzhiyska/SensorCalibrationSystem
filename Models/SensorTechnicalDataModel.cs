namespace SensorCalibrationSystem.Models
{
    /// <summary>
    /// Represents a sensor technical data model.
    /// </summary>
    public class SensorTechnicalDataModel
    {
        /// <summary>
        /// Gets or sets the technical data's parameter.
        /// </summary>
        public string Parameter { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the technical data.
        /// </summary>
        public string Data { get; set; } = string.Empty;

        public SensorTechnicalDataModel(string parameter, string data)
        {
            Parameter = parameter;
            Data = data;
        }
    }
}
