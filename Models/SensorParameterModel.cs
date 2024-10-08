namespace SensorCalibrationSystem.Models
{
    public class SensorParameterModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Value { get; set; } = string.Empty;

        public string Access { get; set; } // e.g., "read-only", "write-only", "read-write"

        public string ParameterId { get; set; }
    }
}
