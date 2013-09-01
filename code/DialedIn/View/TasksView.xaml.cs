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
using DialedIn.ViewModel;
using DialedIn.Model;

// TASKS OVERVIEW UI DEFINITION

namespace DialedIn.View
{
    public partial class TasksView : PhoneApplicationPage
    {
        public TasksView()
        {
            InitializeComponent();
        }

        // event handler for new screen navigated to system event
        // this is where the screen is set-up and the list of application model data is bound to the UI elements
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            App.vm.GetGroups();

            TasksPivotTitle.Title = App.vm.selectedGroup.Title;

            // task can only be selected when user views it in the taskdetailsview
            App.vm.selectedTask = null;

            // LINQ Query on collection of objects:
            // display all active tasks assigned to the logged-in user
            MyTasksListBox.DataContext = from Task in App.vm.selectedGroup.Tasks where Task.IsActive == true && Task.AssignedTo == AppInstance.CurrentUser select Task;
            
            // LINQ Query on collection of objects:
            // display all active tasks
            AllTasksListBox.DataContext = from Task in App.vm.selectedGroup.Tasks where Task.IsActive == true select Task;

            // LINQ Query on collection of objects:
            // display all notifications
            NotificationsListBox.DataContext = from Notification in App.vm.Notifications where Notification.isActive == true select Notification;
        }

        // event handlers for button clicks
        //   specifically navigation controls
        private void appbar_button1_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/TasksEditView.xaml", UriKind.RelativeOrAbsolute));
        }

        private void TextBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            TextBlock senderTB = (TextBlock)sender;

            NavigationService.Navigate(new Uri("/View/TasksDetailView.xaml?msg=" + senderTB.Text, UriKind.Relative));
        }

        private void Groups_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/GroupsView.xaml", UriKind.RelativeOrAbsolute));
        }

        // event handlers for the notification system
        private void TextBlock_MouseEnter_1(object sender, MouseEventArgs e)
        {
            TextBlock senderTB = (TextBlock)sender;
            var notification = from Notification in App.vm.Notifications where Notification.MessageInfo == senderTB.Text select Notification;

            // clear view notificiations
            foreach (Notification n in notification)
            {
                n.isActive = false;
            }
        }

        private void TasksPivotTitle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TasksPivotTitle.SelectedIndex == 2)
            {
                // reset the data context to only active handlers
                NotificationsListBox.DataContext = from Notification in App.vm.Notifications where Notification.isActive == true select Notification;
            }
        }
    }
}