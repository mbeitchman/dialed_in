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
using System.Windows.Media.Imaging;
using Microsoft.Phone.Tasks;
using DialedIn.ViewModel;
using DialedIn.Model;

// TASKS EDIT UI DEFINITION

namespace DialedIn.View
{
    public partial class TasksAddView : PhoneApplicationPage
    {
        // use system chooser components to access the camera and the address book from the phone
        private CameraCaptureTask cameraCaptureTask;
        private EmailAddressChooserTask emailChooserTask;
        public DateTime DT;

        public TasksAddView()
        {
            InitializeComponent();

            DT = DateTime.Now;
            DT = DT.AddDays(1);
            string format = "M/d/yyyy";
            ddtbl.Text = DT.ToString(format);

            // initialize camera chooser and add an event handler
            cameraCaptureTask = new CameraCaptureTask();
            cameraCaptureTask.Completed += new EventHandler<PhotoResult>(cameraCaptureTask_Completed);

            // initialize email chooser and add an event handler
            emailChooserTask = new EmailAddressChooserTask();
            emailChooserTask.Completed += new EventHandler<EmailResult>(emailAddressChooserTask_Completed);
        }

        // event handler set-up initial screen with data
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            PageTitle.Text = App.vm.selectedGroup.Title;

            if (App.vm.selectedTask != null)
            {
                TitleTextBox.Text = App.vm.selectedTask.Title;
                AssignedToTextBox.Text = App.vm.selectedTask.AssignedTo;
                LocationTextBox.Text = App.vm.selectedTask.Location;
                DescriptionTextBox.Text = App.vm.selectedTask.Description;
                
                if(App.vm.selectedTask.TaskImage != null)
                {
                    TaskImage.Source = App.vm.selectedTask.TaskImage;
                }
            }
        }

        // click event handler for save task button
        // saves tasks and edits the screen
        private void SaveButton_Click(object sender, EventArgs e)
        {
            // TODO: if task title is not empty and assigned, if they are, pop up a dialogue?
            if (App.vm.selectedTask != null)
            {
                App.vm.selectedTask.Title = TitleTextBox.Text;
                App.vm.selectedTask.AssignedTo = AssignedToTextBox.Text;
                App.vm.selectedTask.Location = LocationTextBox.Text;
                App.vm.selectedTask.Description = DescriptionTextBox.Text;
                App.vm.selectedTask.DueDate = ddtbl.Text;

                if (TaskImage.Source != null)
                {
                    App.vm.selectedTask.TaskImage = (BitmapImage)TaskImage.Source;
                }
            }
            else
            {
                App.vm.selectedGroup.AddTask(new Task(TitleTextBox.Text, AssignedToTextBox.Text, DescriptionTextBox.Text, (BitmapImage)TaskImage.Source, ddtbl.Text, LocationTextBox.Text));
            }

            NavigationService.Navigate(new Uri("/View/TasksView.xaml", UriKind.RelativeOrAbsolute));
        }

        // click event handler for camera button to launch the device's camera capture experience
        private void PictureButton_Click_1(object sender, EventArgs e)
        {
            try
            {
                cameraCaptureTask.Show();
            }
            catch (System.InvalidOperationException)
            {
                return;
            }
        }

        // event handler to save the captured image to the task
        void cameraCaptureTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                BitmapImage bmp = new BitmapImage();
                bmp.SetSource(e.ChosenPhoto);
                TaskImage.Source = bmp;
            }
        }

        // click event handler to open the phone's address book
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                emailChooserTask.Show();
            }
            catch (System.InvalidOperationException)
            {
                // catch but dont handle the exception
            }
        }

        // event handler to save the selected email address to the task.
        void emailAddressChooserTask_Completed(object sender, EmailResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                AssignedToTextBox.Text += e.Email + ";";
            }
        }

        // click event handler for add days button to set the deadline
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            DT = DT.AddDays(1);
            string format = "M/d/yyyy";
            ddtbl.Text = DT.Date.ToString(format);
        }
    }
}