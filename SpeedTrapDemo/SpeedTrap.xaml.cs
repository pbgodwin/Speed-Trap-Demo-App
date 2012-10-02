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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace SpeedTrapDemo
{
    public sealed partial class SpeedTrap : UserControl
    {
        public SpeedTrap(SpeedTrapLocation location)
        {
            this.InitializeComponent();

            latText.Text = location.Latitude.ToString();
            longText.Text = location.Longitude.ToString();
            //Remember that CreatedAt was a nullable type? We have to make sure there is a value in here, just to be safe
            if (location.CreatedAt.HasValue)
            {
                datePanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
                dateText.Text = location.CreatedAt.Value.ToString();
            }
        }
    }
}
