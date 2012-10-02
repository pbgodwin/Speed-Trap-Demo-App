using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Windows.Devices.Geolocation;
using Bing.Maps;
using Callisto.Controls;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SpeedTrapDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private IMobileServiceTable<SpeedTrapLocation> speedTrapLocationTable;

        public MainPage()
        {
            this.InitializeComponent();

            speedTrapLocationTable = App.mobileServiceClient.GetTable<SpeedTrapLocation>();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            Geolocator geolocator = new Geolocator();
            if (geolocator.LocationStatus != PositionStatus.Disabled)
            {
                try
                {
                    //Use the Location service to fetch the users position and move to it on the map
                    var pos = await geolocator.GetGeopositionAsync();
                    var startLocation = new Location(pos.Coordinate.Latitude, pos.Coordinate.Longitude);
                    SpeedTrapMap.SetView(startLocation);
                }
                catch
                {
                    //If we don't have access to location services, default to this arbitrary location
                    SpeedTrapMap.SetView(new Location(40.2749884116557, -97.4492259995741), 5);
                }
            }
        }

        protected override async void OnGotFocus(RoutedEventArgs e)
        {
            //Make it so that we refresh the map once the app gets focus. 
            //TODO: Find a way to refresh the app without making an API call everytime OnGotFocus is called
            await fetchSpeedTraps();

            base.OnGotFocus(e);
        }

        private void map_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var pos = e.GetPosition(SpeedTrapMap);
            Location location;
            //Take the tapped location and get a GPS coordinate pair from it using the Bing Maps API
            SpeedTrapMap.TryPixelToLocation(pos, out location);

            //Flyout control from Callisto. Very handy.
            Flyout f = new Flyout();

            // Flyout is a ContentControl so set your content within it.
            f.Content = new NewSpeedTrap(location.Longitude, location.Latitude);

            f.Placement = PlacementMode.Top;
            f.PlacementTarget = layoutRoot; // this is an UI element (usually the sender)

            layoutRoot.Children.Add(f.HostPopup);

            f.Closed += (async (b, c) =>
            {
                layoutRoot.Children.Remove(f.HostPopup);
                //Refresh the speed trap locations
                await fetchSpeedTraps();
            });

            f.IsOpen = true;
        }

        private async Task fetchSpeedTraps()
        {
            SpeedTrapMap.Children.Clear(); //Clear all current entries from the map

            //TODO: Replace .Take(1000) with something that handles paging
            var items = await speedTrapLocationTable.Take(1000).ToListAsync();

            foreach (var item in items)
            {
                var pushpin = new Bing.Maps.Pushpin();

                Flyout f = new Flyout();

                pushpin.PointerEntered += ((pinControl, ev) =>
                {


                    // Flyout is a ContentControl so set your content within it.
                    f.Content = new SpeedTrap(item);

                    f.Placement = PlacementMode.Top;
                    f.PlacementTarget = (UIElement)pinControl; //Make it appear near the push pin element

                    layoutRoot.Children.Add(f.HostPopup);

                    f.Closed += (b, c) =>
                    {
                        layoutRoot.Children.Remove(f.HostPopup);
                    };

                    f.IsOpen = true;
                });

                pushpin.PointerExited += ((s, ev) =>
                {
                    f.IsOpen = false;
                });

                //This function orients the pushpin on the map before we add it to the map
                MapLayer.SetPosition(pushpin, new Location(item.Latitude, item.Longitude));
                SpeedTrapMap.Children.Add((UIElement)pushpin);
            }
        }

        private async void refreshAppBarClick(object sender, RoutedEventArgs e)
        {
            //Called when the refresh app bar button is clicked
            await fetchSpeedTraps();
        }

    }
}
