using SensorCalibrationSystem.Models;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;

namespace SensorCalibrationSystem.Views
{
    /// <summary>
    /// Interaction logic for HardwareSetupPage.xaml
    /// </summary>
    public partial class HardwareSetupPage : Page
    {
        private const string appResourcesPath = @"pack://application:,,,/SensorCalibrationSystem;component/Resources";
        private BitmapImage currentBoardDisplayImage = new BitmapImage(new Uri($"{appResourcesPath}/board-front.png"));

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

                BoardDisplay.Source = new BitmapImage(new Uri($"{appResourcesPath}/{imageName}"));
                ChangeBoardDisplayOptionsPanel.Visibility = Visibility.Hidden;
            }
            else
            {
                BoardDisplay.Source = currentBoardDisplayImage; 
                ChangeBoardDisplayOptionsPanel.Visibility = Visibility.Visible;
            }
        }

        private void FrontBoardViewButton_Click(object sender, RoutedEventArgs e)
        {
            currentBoardDisplayImage = new BitmapImage(new Uri($"{appResourcesPath}/board-front.png"));
            BoardDisplay.Source = currentBoardDisplayImage;
        }

        private void BackBoardViewButton_Click(object sender, RoutedEventArgs e)
        {
            currentBoardDisplayImage = new BitmapImage(new Uri($"{appResourcesPath}/board-back.png"));
            BoardDisplay.Source = currentBoardDisplayImage;
        }

        private void PinoutBoardViewButton_Click(object sender, RoutedEventArgs e)
        {
            currentBoardDisplayImage = new BitmapImage(new Uri($"{appResourcesPath}/board-pinout.png"));
            BoardDisplay.Source = currentBoardDisplayImage;
        }
    }
}
