using System;
using System.Globalization;
using System.Windows.Data;

namespace SensorCalibrationSystem.Converters
{
    public class ValueFormatConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 3
                && values[0] is string format
                && values[1] is string input
                && int.TryParse(input, out int value)
                && values[2] is int size)
            {
                switch (format)
                {
                    case "Decimal":
                        return value.ToString("D");
                    case "Hex":
                        return $"0x{value.ToString($"X{size / 4}")}";
                    case "Binary":
                        return value.ToString($"B{size}");
                    default:
                        return string.Empty;
                }
            }

            return string.Empty;
        }

        public object[]? ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
