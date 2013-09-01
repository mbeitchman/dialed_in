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
using Microsoft.Phone.Shell;

// TASKS DETAILS UI DEFINITION-->

namespace DialedIn.View
{
    public partial class TaskDetailsView : PhoneApplicationPage
    {

        public TaskDetailsView()
        {
            InitializeComponent();
        }

        // set-up the initial application screen with requred data
        // this screen parses the task to display's title from the NavigationContext object
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            PageTitle.Text = App.vm.selectedGroup.Title;

            string taskTitle;

            if (NavigationContext.QueryString.TryGetValue("msg", out taskTitle))
            {
                var task = from Task in App.vm.selectedGroup.Tasks where Task.Title == taskTitle select Task;

                App.vm.SetSelectedTask(taskTitle);

                TaskDescriptionBlock.Text = App.vm.selectedTask.Description;
                TaskTitleTextBlock.Text = App.vm.selectedTask.Title;
                AssignedToTextBlock.Text = App.vm.selectedTask.AssignedTo;
                LocationTextBlock.Text = App.vm.selectedTask.Location;

                DueDateTBo.Text = App.vm.selectedTask.DueDate;
                taskImage.Source = App.vm.selectedTask.TaskImage;
            }
            else
            {
                TaskDescriptionBlock.Text = "Error";
                TaskTitleTextBlock.Text = "Error";
                taskImage.Source = null;
            }
        }

        // click event handler for edit button
        // launches edit screen for the task
        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/TasksEditView.xaml", UriKind.Relative));
        }

        // click event handler for completed button
        // sets the task to completed (isActive == false)
        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {   
            if (App.vm.selectedTask.AssignedTo == AppInstance.CurrentUser)
            {
                Notification n = new Notification("Task: " + App.vm.selectedTask.Title + " Completed!");
                App.vm.AddNotifcation(n);
                App.vm.selectedTask.IsActive = false;
            }
            
            NavigationService.Navigate(new Uri("/View/TasksView.xaml", UriKind.Relative));
        }

        private void map_button_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/TaskMapView.xaml", UriKind.Relative));
        }
    }
}