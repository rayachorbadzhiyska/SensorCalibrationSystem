using Microsoft.Extensions.DependencyInjection;
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

            services.AddSingleton<PrintedCircuitBoard3DViewModel>();
            services.AddSingleton<PrintedCircuitBoard3DView>();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var mainWindow = serviceProvider.GetService<MainWindow>();
            mainWindow?.Show();
        }
    }
}
