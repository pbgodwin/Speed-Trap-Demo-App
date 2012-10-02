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

using Microsoft.WindowsAzure.MobileServices;
using Callisto.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace SpeedTrapDemo
{
    public sealed partial class NewSpeedTrap : UserControl
    {

        private double longitude;
        private double latitude;
        private IMobileServiceTable<SpeedTrapLocation> speedTrapLocationTable;

        public NewSpeedTrap(double longitude, double latitude)
        {
            this.InitializeComponent();

            this.longitude = longitude;
            this.latitude = latitude;

            latText.Text = latitude.ToString();
            longText.Text = longitude.ToString();

            speedTrapLocationTable = App.mobileServiceClient.GetTable<SpeedTrapLocation>();
        }

        private async void Submit_Click(object sender, RoutedEventArgs e)
        {

            var trapLocation = new SpeedTrapLocation()
            {
                Latitude = this.latitude,
                Longitude = this.longitude
            };

            await speedTrapLocationTable.InsertAsync(trapLocation);

            Flyout f = this.Parent as Flyout;
            f.IsOpen = false;

        }


    }
}
