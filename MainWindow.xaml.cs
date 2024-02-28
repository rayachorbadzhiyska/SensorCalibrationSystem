using SensorCalibrationSystem.Contracts;
using SensorCalibrationSystem.ViewModels;
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
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            boardCommunicationService.Disconnect();
        }
    }
}
