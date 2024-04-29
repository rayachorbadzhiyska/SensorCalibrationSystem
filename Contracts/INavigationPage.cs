namespace SensorCalibrationSystem.Contracts
{
    /// <summary>
    /// Represents a navigation page.
    /// </summary>
    public interface INavigationPage
    {
        /// <summary>
        /// Gets or sets the page's name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the page's logo.
        /// </summary>
        string Logo { get; set; }

        /// <summary>
        /// Gets or sets the page's navigation index.
        /// </summary>
        int NavigationIndex { get; set; }

        /// <summary>
        /// Gets or sets whether the page is active.
        /// </summary>
        bool IsActive { get; set; }

        /// <summary>
        /// Executes when the page is navigated to.
        /// </summary>
        void OnNavigatedTo();

        /// <summary>
        /// Executes when the page is navigated from.
        /// </summary>
        void OnNavigatedFrom();
    }
}
