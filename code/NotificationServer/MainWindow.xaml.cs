// NotificationServer - MainWindow.xaml.cs

// Primary functionality for the service.

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
using System.Collections.ObjectModel;
using System.Threading;
using System.IO;
using System.Xml;
using WindowsPhone.PushNotificationManager;
using NotificationServer.Service;

namespace NotificationServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Private variables
        private ObservableCollection<CallbackArgs> trace = new ObservableCollection<CallbackArgs>();
        private NotificationSenderUtility notifier = new NotificationSenderUtility();
        private string[] lastSent = null;
        #endregion

        public MainWindow()
        {
            InitializeComponent();

            InitializeTaskIds();
            InitializeTitles();
            InitializeDescriptions();
            InitializePeople();
            InitializeDueDates();
            InitializeLocations();
            InitializeImageUrls();
            InitializeBooleans();

            Log.ItemsSource = trace;
            RegistrationService.Subscribed += new EventHandler<RegistrationService.SubscriptionEventArgs>(RegistrationService_Subscribed);

        }

        #region Initializations
        // Initialize list of selections available for each task data drop-down
        private void InitializeTaskIds()
        {
            List<string> ids = new List<string>();
            ids.Add("0");
            ids.Add("1");
            ids.Add("2");
            ids.Add("3");
            ids.Add("4");

            cmbTaskId.ItemsSource = ids;
            cmbTaskId.SelectedIndex = 0;
        }

        private void InitializeTitles()
        {
            List<string> taskTitles = new List<string>();
            taskTitles.Add("pick up dry cleaning");
            taskTitles.Add("Title 2");
            taskTitles.Add("Title 3");
            taskTitles.Add("Title 4");

            List<string> groupTitles = new List<string>();
            groupTitles.Add("family");
            groupTitles.Add("Group 2");
            groupTitles.Add("Group 3");
            groupTitles.Add("Group 4");

            cmbTitle.ItemsSource = taskTitles;
            cmbTitle.SelectedIndex = 0;

            cmbGroupTitle.ItemsSource = groupTitles;
            cmbGroupTitle.SelectedIndex = 0;
        }

        private void InitializeDescriptions()
        {
            List<string> descriptions = new List<string>();
            descriptions.Add("my dress");
            descriptions.Add("Description 2");
            descriptions.Add("Description 3");
            descriptions.Add("Description 4");

            cmbDescription.ItemsSource = descriptions;
            cmbDescription.SelectedIndex = 0;
        }

        private void InitializePeople()
        {
            Dictionary<string, string> people = new Dictionary<string, string>();
            people.Add("tiffto@gmail.com", "Tiffany");
            people.Add("dick", "Dick");
            people.Add("bojanam@gmail.com", "Bojana Duke");
            people.Add("mbeitchman@gmail.com", "Marc Beitchman");

            cmbAssignedTo.ItemsSource = people;
            cmbAssignedTo.DisplayMemberPath = "Value";
            cmbAssignedTo.SelectedValuePath = "Key";
            cmbAssignedTo.SelectedIndex = 0;

            cmbLastModifiedBy.ItemsSource = people;
            cmbLastModifiedBy.DisplayMemberPath = "Value";
            cmbLastModifiedBy.SelectedValuePath = "Key";
            cmbLastModifiedBy.SelectedIndex = 0;
        }

        private void InitializeDueDates()
        {
            List<string> dueDates = new List<string>();
            dueDates.Add("none");
            dueDates.Add("Monday");
            dueDates.Add("3/14/11");

            cmbDueDate.ItemsSource = dueDates;
            cmbDueDate.SelectedIndex = 0;
        }

        private void InitializeLocations()
        {
            List<string> locations = new List<string>();
            locations.Add("Home");
            locations.Add("Grocery Store");
            locations.Add("flower dry cleaning");
            locations.Add("Work");

            cmbLocation.ItemsSource = locations;
            cmbLocation.SelectedIndex = 0;
        }
        
        private void InitializeImageUrls()
        {
            Dictionary<string, string> images = new Dictionary<string, string>();
            images.Add("URL_1", "Image 1");
            images.Add("URL_2", "Image 2");
            images.Add("URL_3", "Image 3");
            images.Add("URL_4", "Image 4");

            cmbImageUrl.ItemsSource = images;
            cmbImageUrl.DisplayMemberPath = "Value";
            cmbImageUrl.SelectedValuePath = "Key";
            cmbImageUrl.SelectedIndex = 0;
        }

        private void InitializeBooleans()
        {
            List<string> boolValues = new List<string>();
            boolValues.Add("true");
            boolValues.Add("false");

            cmbTaskFiltered.ItemsSource = boolValues;
            cmbTaskFiltered.SelectedIndex = 0;

            cmbTaskCompleted.ItemsSource = boolValues;
            cmbTaskCompleted.SelectedIndex = 1;
        }
        #endregion

        #region Event Handlers
        // Handler for click on the "Send" button
        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
          if ((bool)rbnTile.IsChecked) sendTile();
          else if ((bool)rbnHttp.IsChecked) sendHttp();
          else if ((bool)rbnToast.IsChecked) sendToast();
        }

        // Handler for new subscriber
        void RegistrationService_Subscribed(object sender, RegistrationService.SubscriptionEventArgs e)
        {
            // Check previous notifications, and resend last one to connected client
            if (null != lastSent)
            {
                string taskId = lastSent[0];
                string groupTitle = lastSent[1];
                string title = lastSent[2];
                string description = lastSent[3];
                string assignedTo = lastSent[4];
                string dueDate = lastSent[5];
                string location = lastSent[6];
                string imageUrl = lastSent[7];
                string lastModifiedBy = lastSent[8];
                string taskFiltered = lastSent[9];
                string taskCompleted = lastSent[10];

                List<Uri> subscribers = new List<Uri>();
                subscribers.Add(e.ChannelUri);
                byte[] payload = prepareRAWPayload(
                    taskId,
                    groupTitle,
                    title,
                    description,
                    assignedTo,
                    dueDate,
                    location,
                    imageUrl,
                    lastModifiedBy,
                    taskFiltered,
                    taskCompleted);

                ThreadPool.QueueUserWorkItem((unused) => notifier.SendRawNotification(subscribers, payload, OnMessageSent));
            }

            // Update UI to indicate subscriber has been added
            Dispatcher.BeginInvoke((Action)(() =>
            { UpdateStatus(); })
            );
        }

        // Callback for after notification is sent
        private void OnMessageSent(CallbackArgs response)
        {
            // Log response from the notification sender utility
            Dispatcher.BeginInvoke((Action)(() => { trace.Add(response); }));
        }

        // Send a toast notification
        private void sendToast()
        {
            string msg = txtToastMessage.Text;
            txtToastMessage.Text = "";
            List<Uri> subscribers = RegistrationService.GetSubscribers();
            ThreadPool.QueueUserWorkItem((unused) => notifier.SendToastNotification(subscribers,
                "DIALED IN", msg, OnMessageSent));
        }

        // Send a live tile notification
        private void sendTile()
        {
            int counter = (int)sldCounter.Value;
            List<Uri> subscribers = RegistrationService.GetSubscribers();
            ThreadPool.QueueUserWorkItem((unused) => notifier.SendTileNotification(subscribers, "PushNotificationsToken", counter, "Dialed In", OnMessageSent));
        }

        // Send a raw HTTP notification
        private void sendHttp()
        {
            // Get the list of subscribed WP7 clients
            List<Uri> subscribers = RegistrationService.GetSubscribers();

            // Prepare payload
            byte[] payload = prepareRAWPayload(
                cmbTaskId.SelectedValue as string,
                cmbGroupTitle.SelectedValue as string,
                cmbTitle.SelectedValue as string,
                cmbDescription.SelectedValue as string,
                cmbAssignedTo.SelectedValue as string,
                cmbDueDate.SelectedValue as string,
                cmbLocation.SelectedValue as string,
                cmbImageUrl.SelectedValue as string,
                cmbLastModifiedBy.SelectedValue as string,
                cmbTaskFiltered.SelectedValue as string,
                cmbTaskCompleted.SelectedValue as string);

            // Invoke sending logic asynchronously
            ThreadPool.QueueUserWorkItem((unused) => notifier.SendRawNotification(subscribers,
                                    payload,
                                    OnMessageSent)
                );

            // Save last raw notification for future usage
            lastSent = new string[11];
            lastSent[0] = cmbTaskId.SelectedValue as string;
            lastSent[1] = cmbGroupTitle.SelectedValue as string;
            lastSent[2] = cmbTitle.SelectedValue as string;
            lastSent[3] = cmbDescription.SelectedValue as string;
            lastSent[4] = cmbAssignedTo.SelectedValue as string;
            lastSent[5] = cmbDueDate.SelectedValue as string;
            lastSent[6] = cmbLocation.SelectedValue as string;
            lastSent[7] = cmbImageUrl.SelectedValue as string;
            lastSent[8] = cmbLastModifiedBy.SelectedValue as string;
            lastSent[9] = cmbTaskFiltered.SelectedValue as string;
            lastSent[10] = cmbTaskCompleted.SelectedValue as string;
        }
        #endregion

        #region Private functionality
        // Update status of the web service (indicated by whether there are active subscribers) in the UI
        private void UpdateStatus()
        {
            int activeSubscribers = RegistrationService.GetSubscribers().Count;
            bool isReady = (activeSubscribers > 0);
            txtActiveConnections.Text = activeSubscribers.ToString();
            txtStatus.Text = isReady ? "Ready" : "Waiting for connection...";
        }

        // Prepare XML payload for a toast notification - XML format defined by Dialed In
        private static byte[] prepareRAWPayload(string taskId, string groupTitle, string title, string description, string assignedTo, string dueDate, string location, string imageUrl, string lastModifiedBy, string taskFiltered, string taskCompleted)
        {
            MemoryStream stream = new MemoryStream();

            XmlWriterSettings settings = new XmlWriterSettings() { Indent = true, Encoding = Encoding.UTF8 };
            XmlWriter writer = XmlTextWriter.Create(stream, settings);

            writer.WriteStartDocument();
            writer.WriteStartElement("TaskData");

            writer.WriteStartElement("TaskId");
            writer.WriteValue(taskId);
            writer.WriteEndElement();

            writer.WriteStartElement("GroupTitle");
            writer.WriteValue(groupTitle);
            writer.WriteEndElement();

            writer.WriteStartElement("TaskTitle");
            writer.WriteValue(title);
            writer.WriteEndElement();

            writer.WriteStartElement("TaskDescription");
            writer.WriteValue(description);
            writer.WriteEndElement();

            writer.WriteStartElement("TaskAssignedTo");
            writer.WriteValue(assignedTo);
            writer.WriteEndElement();

            writer.WriteStartElement("TaskDueDate");
            writer.WriteValue(dueDate);
            writer.WriteEndElement();

            writer.WriteStartElement("TaskLocation");
            writer.WriteValue(location);
            writer.WriteEndElement();

            writer.WriteStartElement("TaskImageUrl");
            writer.WriteValue(imageUrl);
            writer.WriteEndElement();

            writer.WriteStartElement("LastModifiedBy");
            writer.WriteValue(lastModifiedBy);
            writer.WriteEndElement();

            writer.WriteStartElement("LastModifiedTime");
            writer.WriteValue(DateTime.Now.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("TaskFiltered");
            writer.WriteValue(taskFiltered);
            writer.WriteEndElement();

            writer.WriteStartElement("TaskCompleted");
            writer.WriteValue(taskCompleted);
            writer.WriteEndElement();

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();

            byte[] payload = stream.ToArray();
            return payload;
        }
        #endregion
    }
}