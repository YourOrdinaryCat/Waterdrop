using System;
using Waterdrop.Common;
using Waterdrop.Data;
using Windows.ApplicationModel.Resources;
using Windows.Graphics.Display;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Waterdrop
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class HubPage : Page
    {
        private readonly NavigationHelper navigationHelper;
        private readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView("Resources");

        public HubPage()
        {
            this.InitializeComponent();

            // Hub is only supported in Portrait orientation
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;

            this.NavigationCacheMode = NavigationCacheMode.Required;

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// </summary>
        public HydrationDataSource SourceViewModel => App.MainHydrationSource;

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            // TODO: Add hydration related tips here.
            // var sampleDataGroups = await SampleDataSource.GetGroupsAsync();
            // this.DefaultViewModel["Groups"] = sampleDataGroups;
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            // TODO: Save the unique state of the page here.
        }

        /// <summary>
        /// Shows the details of an item clicked on in the <see cref="ItemPage"/>
        /// </summary>
        private void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            var itemId = ((Days)e.ClickedItem).Id;
            if (!Frame.Navigate(typeof(DayPage), itemId))
            {
                throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private async void Drink_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (SourceViewModel.AllDays[0].Progress == 100)
            {
                var messageDialog = new MessageDialog(resourceLoader.GetString("ExceedWarning"));

                // Add commands.
                messageDialog.Commands.Add(new UICommand(resourceLoader.GetString("Cancel"),
                    DrinkCommandInvokedHandler, 0));
                messageDialog.Commands.Add(new UICommand(resourceLoader.GetString("Continue"),
                    DrinkCommandInvokedHandler, 1));

                // Set the command indexes.
                messageDialog.DefaultCommandIndex = 0;
                messageDialog.CancelCommandIndex = 1;

                // Show the dialog.
                await messageDialog.ShowAsync();
                return;
            }

            SourceViewModel.DrinkWater(200);
        }

        private void DrinkCommandInvokedHandler(IUICommand command)
        {
            if ((int)command.Id == 1)
            {
                SourceViewModel.DrinkWater(200);
            }
        }

        private void AddDay_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            SourceViewModel.UpsertDay(new Days(DateTime.Now, 2000));
        }
    }
}
