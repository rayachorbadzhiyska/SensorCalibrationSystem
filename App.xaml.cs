using Microsoft.Extensions.DependencyInjection;
using SensorCalibrationSystem.Contracts;
using SensorCalibrationSystem.Services;
using SensorCalibrationSystem.ViewModels;
using SensorCalibrationSystem.Views;
using System;
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

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var mainWindow = serviceProvider.GetService<MainWindow>();
            mainWindow?.Show();
        }
    }
}
