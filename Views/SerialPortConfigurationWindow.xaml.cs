using SensorCalibrationSystem.ViewModels;
using System;
using System.Windows;

namespace SensorCalibrationSystem.Views
{
    /// <summary>
    /// Interaction logic for SerialPortConfigurationWindow.xaml
    /// </summary>
    public partial class SerialPortConfigurationWindow : Window
    {
        private readonly MainWindow mainWindow;

        public SerialPortConfigurationWindow(SerialPortConfigurationViewModel viewModel, MainWindow mainWindow)
        {
            InitializeComponent();
            DataContext = viewModel;
            this.mainWindow = mainWindow;

            viewModel.OnSuccessfullyConfigured += SerialPort_OnSuccessfullyConfigured;
        }

        private void SerialPort_OnSuccessfullyConfigured(object? sender, EventArgs e)
        {
            mainWindow?.Show();

            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!mainWindow.IsActive)
            {
                Application.Current.Shutdown();
            }
        }
    }
}
