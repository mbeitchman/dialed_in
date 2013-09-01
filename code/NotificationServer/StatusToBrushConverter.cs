// NotificationServer - StatusToBrushConverter.cs

// Helper class to change text color for various status messages in the notification history log.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Globalization;
using System.Windows.Data;

namespace NotificationServer
{
    [ValueConversion(typeof(string), typeof(Brush))]
    public class StatusToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((string)value)
            {
                // Subscription status    
                case "Active": return Brushes.Green;
                case "Expired": return Brushes.Red;

                // Notification status
                case "Received": return Brushes.Green;
                case "QueueFull": return Brushes.Orange;
                case "Dropped": return Brushes.Red;

                // Device connection status
                case "Connected": return Brushes.Green;
                case "InActive": return Brushes.Blue;
                case "TempDisconnect": return Brushes.Orange;
                case "Disconnect": return Brushes.Red;

                default: return Brushes.Black;
            }
        }

        // ConvertBack must be defined as an extension of IValueConverter, but we do not implement it
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}