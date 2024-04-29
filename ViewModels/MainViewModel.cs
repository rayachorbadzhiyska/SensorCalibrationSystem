using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SensorCalibrationSystem.Contracts;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SensorCalibrationSystem.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        #region Fields

        private INavigationPage selectedPage;

        #endregion

        #region Properties

        public INavigationPage SelectedPage
        {
            get => selectedPage;
            set
            {
                // Navigate from the previously selected page (if there is any).
                if (selectedPage is not null)
                {
                    selectedPage.OnNavigatedFrom();
                    selectedPage.IsActive = false;
                }

                value.IsActive = true;

                SetProperty(ref selectedPage, value);

                // Navigate to the newly selected page.
                selectedPage.OnNavigatedTo();
            }
        }

        public ObservableCollection<INavigationPage> NavigationPages { get; }

        #endregion

        #region Commands

        public IRelayCommand OnLoadedCommand { get; }

        public IRelayCommand ShowPageCommand { get; }

        #endregion

        #region Constructor

        public MainViewModel(IEnumerable<INavigationPage> navigationPages)
        {
            OnLoadedCommand = new RelayCommand(OnLoaded);
            ShowPageCommand = new RelayCommand<int>(ShowPage);

            NavigationPages = new ObservableCollection<INavigationPage>(navigationPages);

            int index = 0;

            foreach (var page in navigationPages)
            {
                page.NavigationIndex = index++;
            }
        }

        #endregion

        #region Methods

        private void OnLoaded()
        {
            if (NavigationPages.Count > 0)
            {
                SelectedPage = NavigationPages.First();
            }
        }

        private void ShowPage(int index)
        {
            SelectedPage = NavigationPages[index];
        }

        #endregion
    }
}
