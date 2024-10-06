using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SensorCalibrationSystem.Contracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;

namespace SensorCalibrationSystem.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        #region Fields

        private INavigationPage? selectedPage;
        private DispatcherTimer refreshTimer;

        #endregion

        #region Properties

        public INavigationPage? SelectedPage
        {
            get => selectedPage;
            set => SetProperty(ref selectedPage, value);
        }

        public ObservableCollection<INavigationPage> NavigationPages { get; }

        public SerialPortConfigurationViewModel SerialPortConfiguration { get; }

        #endregion

        #region Commands

        public IRelayCommand OnLoadedCommand { get; }

        public IRelayCommand OnUnloadedCommand { get; }

        public IRelayCommand ShowPageCommand { get; }

        #endregion

        #region Constructor

        public MainViewModel(
            IBoardCommunicationService boardCommunicationService,
            SerialPortConfigurationViewModel serialPortConfigurationViewModel,
            IEnumerable<INavigationPage> navigationPages)
        {
            OnLoadedCommand = new RelayCommand(OnLoaded);
            OnUnloadedCommand = new RelayCommand(OnUnloaded);
            ShowPageCommand = new RelayCommand<int>(ShowPage);

            NavigationPages = new ObservableCollection<INavigationPage>(navigationPages);

            int index = 0;

            foreach (var page in navigationPages)
            {
                page.NavigationIndex = index++;
            }

            if (NavigationPages.Count > 0)
            {
                NavigationPages.First().IsActive = true;
                SelectedPage = NavigationPages.First();
            }

            SerialPortConfiguration = serialPortConfigurationViewModel;

            refreshTimer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(1) };
            refreshTimer.Tick += (sender, e) =>
            {
                SerialPortConfiguration.CheckConnection();
            };
        }

        #endregion

        #region Methods

        private void OnLoaded()
        {
            SerialPortConfiguration.LoadBaudRates();

            // Navigate to the first selected page.
            SelectedPage?.OnNavigatedTo();

            refreshTimer.Start();
        }

        private void OnUnloaded()
        {
            refreshTimer.Stop();
        }

        private void ShowPage(int index)
        {
            // Navigate from the previously selected page (if there is any).
            SelectedPage?.OnNavigatedFrom();

            SelectedPage = NavigationPages[index];

            // Navigate to the newly selected page.
            SelectedPage.OnNavigatedTo();
        }

        #endregion
    }
}
