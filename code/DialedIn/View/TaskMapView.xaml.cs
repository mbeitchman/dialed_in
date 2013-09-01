using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Device.Location;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Maps;

// TASK LOCATION UI DEFINITION

namespace DialedIn.View
{
    public partial class TaskMapView : PhoneApplicationPage
    {
        public TaskMapView()
        {
            InitializeComponent();
        }

        // display location of the current task on a map control
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            PageTitle.Text = App.vm.selectedTask.Title + " Location";
            
            Pushpin pushpin = new Pushpin();
            pushpin.Location = new GeoCoordinate(47.62, -122.36);
            pushpin.Content = PageTitle.Text;
            map1.ZoomLevel = 12;
            map1.Center = pushpin.Location;
            map1.Children.Add(pushpin);
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

    }
}