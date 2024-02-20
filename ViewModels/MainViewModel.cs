using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SensorCalibrationSystem.Contracts;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

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
            set => SetProperty(ref selectedPage, value);
        }

        public ObservableCollection<INavigationPage> NavigationPages { get; set; }

        #endregion

        #region Commands

        public ICommand ShowPageCommand { get; }

        #endregion

        #region Constructor

        public MainViewModel(IEnumerable<INavigationPage> navigationPages)
        {
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
        }

        #endregion

        #region Methods

        private void ShowPage(int index)
        {
            SelectedPage = NavigationPages[index];
        }

        #endregion
    }
}
