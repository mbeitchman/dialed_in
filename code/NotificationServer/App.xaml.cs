// NotificationServer - App.xaml.cs

// Start-up and exit functionality for the service.

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.ServiceModel;
using NotificationServer.Service;
using System.ServiceModel.Description;

namespace NotificationServer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // Object for new instance of the registration service
        ServiceHost host;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Attempt to start the service host
            try
            {
                host = new ServiceHost(typeof(RegistrationService));
                host.Open();
            }
            catch (TimeoutException timeoutException)
            {
                MessageBox.Show(String.Format("The service operation timed out. {0}", timeoutException.Message));
            }
            catch (CommunicationException communicationException)
            {
                MessageBox.Show(String.Format("Could not start service host. {0}", communicationException.Message));
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Attempt to shut down the service host
            if (host != null)
            {
                try
                {
                    host.Close();
                }
                catch (TimeoutException)
                {
                    host.Abort();
                }
                catch (CommunicationException)
                {
                    host.Abort();
                }
            }
            base.OnExit(e);
        }
    }
}