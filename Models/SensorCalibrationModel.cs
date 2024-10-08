using System.Collections.Generic;

namespace SensorCalibrationSystem.Models
{
    public class SensorCalibrationModel
    {
        public string Name { get; set; } = string.Empty;

        public List<string> Types { get; set; } = new List<string>();

        public List<SensorCommandModel> Commands { get; set; } = new List<SensorCommandModel>();

        public List<SensorParameterModel> Parameters { get; set; } = new List<SensorParameterModel>();
    }
}
