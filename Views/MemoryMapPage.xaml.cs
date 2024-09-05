using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace SensorCalibrationSystem.Views
{
    /// <summary>
    /// Interaction logic for MemoryMapPage.xaml
    /// </summary>
    public partial class MemoryMapPage : Page
    {
        public MemoryMapPage()
        {
            InitializeComponent();
        }

        private void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;

            if (comboBox is null)
            {
                return;
            }

            ToggleButton toggleButton = comboBox.Template.FindName("toggleButton", comboBox) as ToggleButton;

            if (toggleButton is null)
            {
                return;
            }

            Border border = toggleButton.Template.FindName("templateRoot", toggleButton) as Border;

            if (border != null)
            {
                border.Background = Brushes.Transparent;
            }
        }
    }
}
