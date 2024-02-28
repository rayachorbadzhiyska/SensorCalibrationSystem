using Microsoft.Extensions.DependencyInjection;
using SensorCalibrationSystem.Contracts;
using SensorCalibrationSystem.Services;
using SensorCalibrationSystem.ViewModels;
using SensorCalibrationSystem.Views;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace SensorCalibrationSystem
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IServiceProvider serviceProvider;

        public App()
        {
            ServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
            serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<MainWindow>();

            services.AddSingleton<SerialPortConfigurationViewModel>();
            services.AddSingleton<SerialPortConfigurationWindow>();

            services.AddSingleton<INavigationPage, HardwareSetupViewModel>();
            services.AddSingleton<HardwareSetupPage>();

            services.AddSingleton<PrintedCircuitBoard3DViewModel>();
            services.AddSingleton<PrintedCircuitBoard3DView>();

            services.AddSingleton<INavigationPage, MemoryMapViewModel>();
            services.AddSingleton<MemoryMapPage>();

            services.AddSingleton<INavigationPage, CalibrationViewModel>();
            services.AddSingleton<CalibrationPage>();

            services.AddSingleton<IBoardCommunicationService, BoardCommunicationService>();
        }

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            await StartServer();

            var mainWindow = serviceProvider.GetService<SerialPortConfigurationWindow>();
            mainWindow?.Show();
        }

        /// <summary>
        /// Starts the server, which hosts the available resources.
        /// </summary>
        /// <returns></returns>
        private async Task StartServer()
        {
            string localServerPort = SensorCalibrationSystem.Resources.Resources.LocalServerPort;

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "python3",
                Arguments = $"-m http.server {localServerPort}",
                WorkingDirectory = Directory.GetCurrentDirectory(),
                CreateNoWindow = true,
                UseShellExecute = false
            };

            Process.Start(psi);

            await Task.CompletedTask;
        }
    }
}
