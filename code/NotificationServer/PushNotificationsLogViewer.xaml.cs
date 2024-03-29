// NotificationServer - PushNotificationsLogViewer.xaml.cs

// Code behind user interface component for logging sent notification history.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections;

namespace NotificationServer
{
    /// <summary>
    /// Interaction logic for PushNotificationsLogViewer.xaml
    /// </summary>
    public partial class PushNotificationsLogViewer : UserControl
    {
        public PushNotificationsLogViewer()
        {
            InitializeComponent();
            tracelog.LostFocus += (sender, e) => { tracelog.SelectedIndex = -1; };
        }

        // ItemsSource must be defined for this extension of UserControl
        public IEnumerable ItemsSource
        {
            get
            {
                return this.tracelog.ItemsSource;
            }

            set
            {
                this.tracelog.ItemsSource = value;
            }
        }
    }
}