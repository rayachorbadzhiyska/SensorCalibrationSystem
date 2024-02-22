using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SensorCalibrationSystem.Models
{
    /// <summary>
    /// Represents a sensor model.
    /// </summary>
    public class SensorModel
    {
        /// <summary>
        /// Gets or sets the sensor's name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the sensor's type.
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the sensor's image's path.
        /// </summary>
        public string ImagePath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the sensor's technical data.
        /// </summary>
        public List<SensorTechnicalDataModel> TechnicalData { get; set; } = new List<SensorTechnicalDataModel>();

        /// <summary>
        /// Gets or sets whether the sensor is currently attached.
        /// </summary>
        [JsonIgnore]
        public bool IsAttached { get; set; }

        [JsonConstructor]
        public SensorModel(string name, string type, string imagePath, List<SensorTechnicalDataModel> technicalData)
        {
            Name = name;
            Type = type;
            ImagePath = @"/SensorCalibrationSystem;component/Resources/" + imagePath;
            TechnicalData = technicalData;
        }
    }
}
