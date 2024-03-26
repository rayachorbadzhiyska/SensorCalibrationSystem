using SensorCalibrationSystem.Contracts;
using SensorCalibrationSystem.ViewModels;
using System;
using System.Windows;

namespace SensorCalibrationSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IBoardCommunicationService boardCommunicationService;

        public MainWindow(MainViewModel viewModel, IBoardCommunicationService boardCommunicationService)
        {
            InitializeComponent();
            DataContext = viewModel;

            this.boardCommunicationService = boardCommunicationService;

            StateChanged += OnWindowStateChanged;

            MaxHeight = SystemParameters.VirtualScreenHeight;
            MaxWidth = SystemParameters.VirtualScreenWidth;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            boardCommunicationService.Disconnect();
        }

        #region Toolbar

        private void OnMinimizeButtonClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void OnMaximizeRestoreButtonClick(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else
            {
                WindowState = WindowState.Maximized;
            }
        }

        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void RefreshMaximizeRestoreButton()
        {
            if (WindowState == WindowState.Maximized)
            {
                maximizeButton.Visibility = Visibility.Collapsed;
                restoreButton.Visibility = Visibility.Visible;
            }
            else
            {
                maximizeButton.Visibility = Visibility.Visible;
                restoreButton.Visibility = Visibility.Collapsed;
            }
        }

        private void OnWindowStateChanged(object? sender, EventArgs e)
        {
            RefreshMaximizeRestoreButton();
        }

        #endregion
    }
}
