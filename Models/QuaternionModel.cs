using System.Globalization;

namespace SensorCalibrationSystem.Models
{
    /// <summary>
    /// Represents a quaternion model.
    /// </summary>
    public class QuaternionModel
    {
        /// <summary>
        /// Gets or sets the x value.
        /// </summary>
        public float XValue { get; set; }

        /// <summary>
        /// Gets or sets the y value.
        /// </summary>
        public float YValue { get; set; }

        /// <summary>
        /// Gets or sets the z value.
        /// </summary>
        public float ZValue { get; set; }

        /// <summary>
        /// Gets or sets the w value.
        /// </summary>
        public float WValue { get; set; }

        public QuaternionModel(float x, float y, float z, float w)
        {
            XValue = x;
            YValue = y;
            ZValue = z;
            WValue = w;
        }

        public override string ToString()
        {
            var culture = CultureInfo.InvariantCulture;
            return $"X: {XValue.ToString(culture)}, " +
                $"Y: {YValue.ToString(culture)}, " +
                $"Z: {ZValue.ToString(culture)}, " +
                $"W: {WValue.ToString(culture)}";
        }
    }
}
