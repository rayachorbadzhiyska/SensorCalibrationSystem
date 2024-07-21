using SensorCalibrationSystem.Models;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace SensorCalibrationSystem.Converters
{
    public class MemoryMapToolTipSourceConverter : IValueConverter
    {
        private const string appResourcesPath = @"pack://application:,,,/SensorCalibrationSystem;component/Resources";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not null && value is MemoryMapModel memoryMap)
            {
                string imageName = $"{memoryMap.Sensor}-memory-map.png";

                return new BitmapImage(new Uri($"{appResourcesPath}/{imageName}"));
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
