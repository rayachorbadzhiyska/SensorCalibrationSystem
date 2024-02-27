using SensorCalibrationSystem.Models;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SensorCalibrationSystem.Views
{
    /// <summary>
    /// Interaction logic for HardwareSetupPage.xaml
    /// </summary>
    public partial class HardwareSetupPage : Page
    {
        private const string appResourcesPath = @"pack://application:,,,/SensorCalibrationSystem;component/Resources";

        public HardwareSetupPage()
        {
            InitializeComponent();
        }

        private void SensorsDisplay_MouseMove(object sender, MouseEventArgs e)
        {
            var item = VisualTreeHelper.HitTest(SensorsDisplay, Mouse.GetPosition(SensorsDisplay)).VisualHit;

            // find ListViewItem (or null)
            while (item != null && !(item is ListBoxItem))
                item = VisualTreeHelper.GetParent(item);

            if (item is not null)
            {
                SensorModel sensor = ((ListBoxItem)item).DataContext as SensorModel;

                if (sensor is null)
                {
                    return;
                }

                string imageName = $"{sensor.Name}-on-board.png";

                BoardDisplay.Source = new BitmapImage(new System.Uri($"{appResourcesPath}/{imageName}"));
            }
            else
            {
                BoardDisplay.Source = new BitmapImage(new System.Uri($"{appResourcesPath}/board-top.png"));
            }
        }
    }
}
