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
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Notification;
using DialedIn.Model;
using System.Diagnostics;
using System.IO;
using System.Xml.Linq;
using System.IO.IsolatedStorage;
using System.Collections.ObjectModel;

// LOG_IN SCREEN UI CODE

namespace DialedIn
{
    public partial class MainPage : PhoneApplicationPage
    {
        private HttpNotificationChannel httpChannel;
        const string channelName = "DialedInChannel";
        const string fileName = "PushNotificationSettings.dat";
        const int pushConnectTimeout = 30;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            if (!TryFindChannel())
                DoConnect();
        }

        // event handler for click event on submit button
        //  stores the username and navigates to tasks overview screen when the user
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            AppInstance.CurrentUser = UsernameTextBox.Text;

            NavigationService.Navigate(new Uri("/View/TasksView.xaml", UriKind.RelativeOrAbsolute));
        }

        // debug output for push notification channel subscription
        #region Debug Tracing
        private void Trace(string message)
        {
#if DEBUG
            Debug.WriteLine(message);
#endif
        }
        #endregion

        #region Manage Subscriptions
        private void SubscribeToChannelEvents()
        {
            // Register to UriUpdated event - occurs when channel successfully opens
            httpChannel.ChannelUriUpdated += new EventHandler<NotificationChannelUriEventArgs>(httpChannel_ChannelUriUpdated);

            // Subscribe to raw notification
            httpChannel.HttpNotificationReceived += new EventHandler<HttpNotificationEventArgs>(httpChannel_HttpNotificationReceived);

            // Subscribe to toast notification when running app    
            httpChannel.ShellToastNotificationReceived += new EventHandler<NotificationEventArgs>(httpChannel_ShellToastNotificationReceived);

            // General error handling for push channel
            httpChannel.ErrorOccurred += new EventHandler<NotificationChannelErrorEventArgs>(httpChannel_ExceptionOccurred);
        }

        private void SubscribeToService()
        {
            // Hardcode for now - needs to be updated if the service is deployed to a live server
            string baseUri = "http://localhost:8000/RegistratorService/Register?uri={0}";
            string theUri = String.Format(baseUri, httpChannel.ChannelUri.ToString());
            WebClient client = new WebClient();
            client.DownloadStringCompleted += (s, e) =>
            {
                if (null == e.Error)
                {
                    Trace("Registration succeeded");
                }
                else
                {
                    Trace("Registration failed: " + e.Error.Message);
                }
            };
            client.DownloadStringAsync(new Uri(theUri));
        }

        private void SubscribeToNotifications()
        {
            // Bind to Toast Notification 
            try
            {
                if (httpChannel.IsShellToastBound == true)
                {
                    Trace("Already bounded (register) to to Toast notification");
                }
                else
                {
                    Trace("Registering to Toast Notifications");
                    httpChannel.BindToShellToast();
                }
            }
            catch (Exception ex)
            {
                // TODO: handle error here
            }

            // Bind to Tile Notification 
            try
            {
                if (httpChannel.IsShellTileBound == true)
                {
                    Trace("Already bounded (register) to Tile Notifications");
                }
                else
                {
                    Trace("Registering to Tile Notifications");

                    Collection<Uri> uris = new Collection<Uri>();
                    httpChannel.BindToShellTile(uris);
                }
            }
            catch (Exception ex)
            {
                // TODO: handle error here
            }
        }
        #endregion

        #region Channel event handlers
        // Subscribe to the received channel
        void httpChannel_ChannelUriUpdated(object sender, NotificationChannelUriEventArgs e)
        {
            Trace("Channel opened. Got Uri: " + httpChannel.ChannelUri.ToString());
            Dispatcher.BeginInvoke(() => SaveChannelInfo());
            Trace("Subscribing to channel events");
            SubscribeToService();
            Trace("Subscribe to the channel to Tile and Toast notifications");
            SubscribeToNotifications();

            Trace("Channel created successfully");
        }

        void httpChannel_ExceptionOccurred(object sender, NotificationChannelErrorEventArgs e)
        {
            Trace(e.ErrorType + " occurred: " + e.Message);
        }

        // Process raw HTTP notification
        void httpChannel_HttpNotificationReceived(object sender, HttpNotificationEventArgs e)
        {
            Group targetGroup = null;
            
            Trace("===============================================");
            Trace("RAW notification arrived:");

            string taskId, groupTitle, taskTitle, description, assignedTo, dueDate, location, imageUrl, modifiedBy, modifiedTime, taskFiltered, taskCompleted;
            ParseRAWPayload(e.Notification.Body, out taskId, out groupTitle, out taskTitle, out description, out assignedTo, out dueDate, out location, out imageUrl, out modifiedBy, out modifiedTime, out taskFiltered, out taskCompleted);
            
            App.vm.GetGroupWithTitle(groupTitle, out targetGroup);
            targetGroup.AddTask(new Task(taskTitle, assignedTo, description, dueDate, location));

            App.vm.AddNotifcation(new Notification("New task " + taskTitle + " added to " + groupTitle));

            Trace("===============================================");
        }

        // Process toast notification
        void httpChannel_ShellToastNotificationReceived(object sender, NotificationEventArgs e)
        {
            Trace("===============================================");
            Trace("Toast/Tile notification arrived:");
            foreach (var key in e.Collection.Keys)
            {
                string msg = e.Collection[key];
                Trace("Toast/Tile message: " + msg);
                App.vm.AddNotifcation(new Notification(msg));
            }

            Trace("===============================================");
        }
        #endregion

        #region Loading/Saving Channel Info
        // Find a channel to subscribe to
        private bool TryFindChannel()
        {
            bool bRes = false;

            Trace("Getting IsolatedStorage for current Application");
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                Trace("Checking channel data");
                if (isf.FileExists(fileName))
                {
                    Trace("Channel data exists! Loading...");
                    using (IsolatedStorageFileStream isfs = new IsolatedStorageFileStream(fileName, FileMode.Open, isf))
                    {
                        using (StreamReader sr = new StreamReader(isfs))
                        {
                            string uri = sr.ReadLine();
                            Trace("Finding channel");
                            httpChannel = HttpNotificationChannel.Find(channelName);

                            if (null != httpChannel)
                            {
                                if (httpChannel.ChannelUri.ToString() == uri)
                                {
                                    Trace("Channel retrieved");
                                    SubscribeToChannelEvents();
                                    SubscribeToService();
                                    bRes = true;
                                }

                                sr.Close();
                            }
                        }
                    }
                }
                else
                    Trace("Channel data not found");
            }

            return bRes;
        }

        // Save info for the channel being subscribed to
        private void SaveChannelInfo()
        {
            Trace("Getting IsolatedStorage for current Application");
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                Trace("Creating data file");
                using (IsolatedStorageFileStream isfs = new IsolatedStorageFileStream(fileName, FileMode.Create, isf))
                {
                    using (StreamWriter sw = new StreamWriter(isfs))
                    {
                        Trace("Saving channel data...");
                        sw.WriteLine(httpChannel.ChannelUri.ToString());
                        sw.Close();
                        Trace("Saving done");
                    }
                }
            }
        }
        #endregion

        #region Misc logic
        // Attempt to connect to the notification server
        private void DoConnect()
        {
            try
            {
                // First, try to pick up existing channel
                httpChannel = HttpNotificationChannel.Find(channelName);

                if (null != httpChannel)
                {
                    Trace("Channel Exists - no need to create a new one");
                    SubscribeToChannelEvents();

                    Trace("Register the URI with 3rd party web service");
                    SubscribeToService();

                    Trace("Subscribe to the channel to Tile and Toast notifications");
                    SubscribeToNotifications();

                    Trace("Channel recovered");
                }
                else
                {
                    Trace("Trying to create a new channel...");
                    // Create the channel
                    httpChannel = new HttpNotificationChannel(channelName, "DialedInNotificationService");
                    Trace("New Push Notification channel created successfully");

                    SubscribeToChannelEvents();

                    Trace("Trying to open the channel");
                    httpChannel.Open();
                    Trace("Channel open requested");
                }
            }
            catch (Exception ex)
            {
                Trace("Channel error: " + ex.Message);
            }
        }

        // Parse payload of the raw HTTP notification for task data
        private void ParseRAWPayload(Stream e, out string taskId, out string groupTitle, out string taskTitle, out string description, 
            out string assignedTo, out string dueDate, out string location, out string imageUrl, out string modifiedBy, out string modifiedTime,
            out string taskFiltered, out string taskCompleted)
        {
            XDocument document;

            using (var reader = new StreamReader(e))
            {
                string payload = reader.ReadToEnd().Replace('\0', ' ');
                document = XDocument.Parse(payload);
            }

            taskId = (from c in document.Descendants("TaskData")
                      select c.Element("TaskId").Value).FirstOrDefault();
            Trace("Got task ID: " + taskId);

            groupTitle = (from c in document.Descendants("TaskData")
                      select c.Element("GroupTitle").Value).FirstOrDefault();
            Trace("Got group title: " + groupTitle);

            taskTitle = (from c in document.Descendants("TaskData")
                      select c.Element("TaskTitle").Value).FirstOrDefault();
            Trace("Got task title: " + taskTitle);

            description = (from c in document.Descendants("TaskData")
                      select c.Element("TaskDescription").Value).FirstOrDefault();
            Trace("Got description: " + description);

            assignedTo = (from c in document.Descendants("TaskData")
                      select c.Element("TaskAssignedTo").Value).FirstOrDefault();
            Trace("Got assigned to: " + assignedTo);

            dueDate = (from c in document.Descendants("TaskData")
                       select c.Element("TaskDueDate").Value).FirstOrDefault();
            Trace("Got due date: " + dueDate);

            location = (from c in document.Descendants("TaskData")
                      select c.Element("TaskLocation").Value).FirstOrDefault();
            Trace("Got location: " + location);

            imageUrl = (from c in document.Descendants("TaskData")
                      select c.Element("TaskImageUrl").Value).FirstOrDefault();
            Trace("Got image URL: " + imageUrl);

            modifiedBy = (from c in document.Descendants("TaskData")
                      select c.Element("LastModifiedBy").Value).FirstOrDefault();
            Trace("Got last modified by: " + modifiedBy);

            modifiedTime = (from c in document.Descendants("TaskData")
                      select c.Element("LastModifiedTime").Value).FirstOrDefault();
            Trace("Got last modified time: " + modifiedTime);

            taskFiltered = (from c in document.Descendants("TaskData")
                      select c.Element("TaskFiltered").Value).FirstOrDefault();
            Trace("Got filtered status: " + taskFiltered);

            taskCompleted = (from c in document.Descendants("TaskData")
                      select c.Element("TaskCompleted").Value).FirstOrDefault();
            Trace("Got completed status: " + taskCompleted);
        }
        #endregion
    }
}