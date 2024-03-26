using SensorCalibrationSystem.Contracts;
using SensorCalibrationSystem.ViewModels;
using System;
using System.ComponentModel;
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

            MaxHeight = SystemParameters.VirtualScreenHeight - 35;
            BorderThickness = new Thickness(6);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            boardCommunicationService.Disconnect();
        }

        #region Toolbar

        private void OnMinimizeButtonClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
            BorderThickness = new Thickness(0);
        }

        private void OnMaximizeRestoreButtonClick(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
                BorderThickness = new Thickness(0);
            }
            else
            {
                WindowState = WindowState.Maximized;
                BorderThickness = new Thickness(6);
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
                BorderThickness = new Thickness(6);
            }
            else
            {
                maximizeButton.Visibility = Visibility.Visible;
                restoreButton.Visibility = Visibility.Collapsed;
                BorderThickness = new Thickness(0);
            }
        }

        private void OnWindowStateChanged(object? sender, EventArgs e)
        {
            RefreshMaximizeRestoreButton();
        }

        #endregion
    }
}
